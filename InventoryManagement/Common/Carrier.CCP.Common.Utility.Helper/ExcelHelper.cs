using Carrier.CCP.Common.Utils.Exception;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Carrier.CCP.Common.Utility.Helper
{
    public class ExcelHelper
    {
        const int DEFAULT_COLUMN_WIDTH = 25;        // Set each column to be 20 wide

        static int getColumnCount(string sheetName)
        {
            switch (sheetName)
            {
                case "Metrics Detail Mapping": return 15;
                case "Metric Discrete Value": return 7;
                case "Metric Discrete Text": return 4;
                default: return -1;
            }
        }

        //Only xlsx files
        public static DataSet GetDataTableFromExcelFile(byte[] fileBytes, int noOfSheetsInValidModel)
        {
            DataSet ds = new DataSet();
            string errorMessage = string.Empty;
            using (var fileStream = new MemoryStream(fileBytes))
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(fileStream, false))
            {
                WorkbookPart wbPart = document.WorkbookPart;
                IEnumerable<Sheet> sheets = document.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                if (sheets.Count() != noOfSheetsInValidModel)
                {
                    errorMessage = "Selected Excel file is invalid";
                    throw new CcpValidationException(errorMessage);
                }
                //string sheetId = sheetName != "" ? sheets.Where(q => q.Name == sheetName).First().Id.Value : sheets.First().Id.Value;
                foreach (var sheet in sheets)
                {
                    string sheetId = sheet.Id.Value;
                    var dt = new DataTable(sheet.Name);
                    WorksheetPart wsPart = (WorksheetPart)wbPart.GetPartById(sheetId);
                    SheetData sheetdata = wsPart.Worksheet.Elements<SheetData>().FirstOrDefault();
                    int totalHeaderCount = sheetdata.Descendants<Row>().ElementAt(0).Descendants<Cell>().Count();
                    //Check for columns count for 18 as per CarrierSmart. sheet1: Metrics Detail Mapping sheet2: Metric Discrete Value, 7 sheet3: Metric Discrete Text, 4
                    if (totalHeaderCount != getColumnCount(sheet.Name))
                    {
                        errorMessage = "Selected Excel file is invalid";
                        throw new CcpValidationException(errorMessage);
                    }
                    //Get the header                    
                    for (int i = 1; i <= totalHeaderCount; i++)
                    {
                        dt.Columns.Add(GetCellValue(wbPart, sheetdata.Descendants<Row>().ElementAt(0).Elements<Cell>().ToList(), i));
                    }

                    foreach (Row r in sheetdata.Descendants<Row>())
                    {
                        if (r.RowIndex > 1)
                        {
                            DataRow tempRow = dt.NewRow();

                            //Always get from the header count, because the index of the row changes where empty cell is not counted
                            for (int i = 1; i <= totalHeaderCount; i++)
                            {
                                tempRow[i - 1] = GetCellValue(wbPart, r.Elements<Cell>().ToList(), i);
                            }
                            dt.Rows.Add(tempRow);
                        }
                    }
                    ds.Tables.Add(dt);
                }
            }

            return ds;
        }

        //To get the value of the cell, even it's empty. Unable to use loop by index
        private static string GetCellValue(WorkbookPart wbPart, List<Cell> theCells, string cellColumnReference)
        {
            Cell theCell = null;
            string value = "";
            foreach (Cell cell in theCells)
            {
                if (cell.CellReference.Value.StartsWith(cellColumnReference))
                {
                    theCell = cell;
                    break;
                }
            }
            if (theCell != null)
            {
                value = theCell.InnerText;
                // If the cell represents an integer number, you are done. 
                // For dates, this code returns the serialized value that represents the date. The code handles strings and 
                // Booleans individually. For shared strings, the code looks up the corresponding value in the shared string table. For Booleans, the code converts the value into the words TRUE or FALSE.
                if (theCell.DataType != null)
                {
                    switch (theCell.DataType.Value)
                    {
                        case CellValues.SharedString:
                            // For shared strings, look up the value in the shared strings table.
                            var stringTable = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                            // If the shared string table is missing, something is wrong. Return the index that is in the cell. Otherwise, look up the correct text in the table.
                            if (stringTable != null)
                            {
                                value = stringTable.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
                            }
                            break;
                        case CellValues.Boolean:
                            switch (value)
                            {
                                case "0":
                                    value = "FALSE";
                                    break;
                                default:
                                    value = "TRUE";
                                    break;
                            }
                            break;
                    }
                }
            }
            return value;
        }

        private static string GetCellValue(WorkbookPart wbPart, List<Cell> theCells, int index)
        {
            return GetCellValue(wbPart, theCells, GetExcelColumnName(index));
        }

        private static string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;
            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }
            return columnName;
        }


        /// <summary>
        /// Create an Excel file, and write it to a file.
        /// </summary>
        /// <param name="ds">DataSet containing the data to be written to the Excel.</param>
        /// <param name="excelFilename">Name of file to be written.</param>
        /// <returns>True if successful, false if something went wrong.</returns>
        public async static Task<MemoryStream> CreateExcelDocument(DataSet ds, List<string[]> metricValidList)
        {
            try
            {
                var documentStream = new MemoryStream();
                using (SpreadsheetDocument spreadsheet = SpreadsheetDocument.Create(documentStream, SpreadsheetDocumentType.Workbook))
                {
                    WriteExcelFile(ds, spreadsheet, metricValidList);
                    spreadsheet.Close();
                }

                documentStream.Seek(0, SeekOrigin.Begin);
                MemoryStream AsMemoryStream = new MemoryStream();
                await documentStream.CopyToAsync(AsMemoryStream);
                return AsMemoryStream;
            }
            catch (Exception ex)
            {
                //TBD: write and throw specific exception
                //Trace.WriteLine("Failed, exception thrown: " + ex.Message);
                return null;
            }
        }

        private static void WriteExcelFile(DataSet ds, SpreadsheetDocument spreadsheet, List<string[]> metricValidList)
        {
            //  Create the Excel file contents.  This function is used when creating an Excel file either writing 
            //  to a file, or writing to a MemoryStream.
            spreadsheet.AddWorkbookPart();
            spreadsheet.WorkbookPart.Workbook = new Workbook();

            DefinedNames definedNamesCol = new DefinedNames();

            //  My thanks to James Miera for the following line of code (which prevents crashes in Excel 2010)
            spreadsheet.WorkbookPart.Workbook.Append(new BookViews(new WorkbookView()));

            //  If we don't add a "WorkbookStylesPart", OLEDB will refuse to connect to this .xlsx file !
            WorkbookStylesPart workbookStylesPart = spreadsheet.WorkbookPart.AddNewPart<WorkbookStylesPart>("rIdStyles");
            workbookStylesPart.Stylesheet = GenerateStyleSheet();
            workbookStylesPart.Stylesheet.Save();


            //  Loop through each of the DataTables in our DataSet, and create a new Excel Worksheet for each.
            uint worksheetNumber = 1;
            Sheets sheets = spreadsheet.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            foreach (DataTable dt in ds.Tables)
            {
                //  For each worksheet you want to create
                string worksheetName = dt.TableName;

                //  Create worksheet part, and add it to the sheets collection in workbook
                WorksheetPart newWorksheetPart = spreadsheet.WorkbookPart.AddNewPart<WorksheetPart>();
                Sheet sheet = new Sheet() { Id = spreadsheet.WorkbookPart.GetIdOfPart(newWorksheetPart), SheetId = worksheetNumber, Name = worksheetName };
                sheets.Append(sheet);

                //  Append this worksheet's data to our Workbook, using OpenXmlWriter, to prevent memory problems
                WriteDataTableToExcelWorksheet(dt, newWorksheetPart, definedNamesCol);

                //SheetData sheetdata = newWorksheetPart.Worksheet.Elements<SheetData>().FirstOrDefault();
                //int totalHeaderCount = sheetdata.Descendants<Row>().ElementAt(0).Descendants<Cell>().Count();
                //for (int i = 1; i <= totalHeaderCount; i++)
                //{
                //    string cell = Enum.GetName(typeof(ColumnRef), i);
                //    AddValidationsToSheet(string.Empty, newWorksheetPart.Worksheet, null, cell.ElementAt(0));
                //}
                if (sheet.Name == MetricDetail_Sheet)
                    AddValidationsToSheet(sheet.Name, newWorksheetPart.Worksheet, metricValidList);

                worksheetNumber++;
            }
            spreadsheet.WorkbookPart.Workbook.Append(definedNamesCol);
            spreadsheet.WorkbookPart.Workbook.Save();
        }

        const string MetricDetail_Sheet = "Metrics Detail Mapping";
        const string MetricDiscreteValue_Sheet = "Metric Discrete Value";
        const string MetricDiscreteText_Sheet = "Metric Discrete Text";
        public enum ColumnRef
        {
            A = 1, B = 2, C = 3, D = 4, E = 5, F = 6, G = 7, H = 8, I = 9, J = 10, K = 11, L = 12, M = 13, N = 14, O = 15, P = 16
        };
        private static void AddValidationsToSheet(string sheetName, Worksheet worksheet, List<string[]> metricValidList, char column = char.MinValue)
        {
            string flatMetricType = string.Empty, flatDataType = string.Empty, flatPermission = string.Empty, flatdesignValue = string.Empty, flatSamplingMode = string.Empty, flatDeviceMainPage = string.Empty, flatSubSystemName = string.Empty;
            DataValidation dataValidation = null;
            DataValidations newDVs = new DataValidations();
            switch (sheetName)
            {
                case MetricDetail_Sheet:
                    flatMetricType = string.Join(",", metricValidList[0]);
                    flatDataType = string.Join(",", metricValidList[1]);
                    flatPermission = string.Join(",", metricValidList[2]);
                    flatdesignValue = string.Join(",", metricValidList[3]);
                    flatSamplingMode = string.Join(",", metricValidList[4]);
                    flatSubSystemName = string.Join(",", metricValidList[5]);
                    break;
                default:
                    //dataValidation = new DataValidation
                    //{
                    //    AllowBlank = false,
                    //    //Use A:A or A1:A1048576 to select the entire column A
                    //    //1048576 (2^20) is the max row number since Excel 2007.
                    //    SequenceOfReferences = new ListValue<StringValue>() { InnerText = $"{column}:{column}" },
                    //};
                    break;
            }
            if (!string.IsNullOrWhiteSpace(flatMetricType))
            {
                dataValidation = ValidationLogic(flatMetricType, "E");
                newDVs.Append(dataValidation);
            }
            if (!string.IsNullOrWhiteSpace(flatDataType))
            {
                dataValidation = ValidationLogic(flatDataType, "H");
                newDVs.Append(dataValidation);
            }
            if (!string.IsNullOrWhiteSpace(flatSubSystemName))
            {
                dataValidation = ValidationLogic(flatSubSystemName, "I");
                newDVs.Append(dataValidation);
            }
            if (!string.IsNullOrWhiteSpace(flatPermission))
            {
                dataValidation = ValidationLogic(flatPermission, "J");
                newDVs.Append(dataValidation);
            }
            if (!string.IsNullOrWhiteSpace(flatdesignValue))
            {
                dataValidation = ValidationLogic(flatdesignValue, "L");
                newDVs.Append(dataValidation);
            }
            if (!string.IsNullOrWhiteSpace(flatSamplingMode))
            {
                dataValidation = ValidationLogic(flatSamplingMode, "M");
                newDVs.Append(dataValidation);
            }

            AddValidationsToWorksheet(worksheet, dataValidation, newDVs);
        }

        private static void AddValidationsToWorksheet(Worksheet worksheet, DataValidation dataValidation, DataValidations newDVs)
        {
            //Check if there are any other DataValidations already in the worksheet
            DataValidations dvs = worksheet.GetFirstChild<DataValidations>();
            if (dvs != null)
            {
                dvs.Count = dvs.Count + 1;
                dvs.Append(dataValidation);
                worksheet.Append(dvs);
            }
            else
            {
                //newDVs.Append(dataValidation);
                //newDVs.Count = 1;
                //Append the validation to the DocumentFormat.OpenXml.SpreadSheet.Worksheet variable
                worksheet.Append(newDVs);
            }
        }

        static DataValidation ValidationLogic(string flatString, string cellColumn)
        {
            return new DataValidation
            {
                Type = DataValidationValues.List,
                AllowBlank = false,
                SequenceOfReferences = new ListValue<StringValue>() { InnerText = $"{cellColumn}:{cellColumn}" },
                Formula1 = new Formula1("\"" + flatString + "\"")
            };
        }

        private static Stylesheet GenerateStyleSheet()
        {
            //  If you want certain Excel cells to have a different Format, color, border, fonts, etc, then you need to define a "CellFormats" records containing 
            //  these attributes, then assign that style number to your cell.
            //
            //  For example, we'll define "Style # 3" with the attributes we'd like for our header row (Row #1) on each worksheet, where the text is a bit bigger,
            //  and is white text on a dark-gray background.
            // 
            //  NB: The NumberFormats from 0 to 163 are hardcoded in Excel (described in the following URL), and we'll define a couple of custom number formats below.
            //  https://msdn.microsoft.com/en-us/library/documentformat.openxml.spreadsheet.numberingformat.aspx
            //  http://lateral8.com/articles/2010/6/11/openxml-sdk-20-formatting-excel-values.aspx
            //
            uint iExcelIndex = 164;

            return new Stylesheet(
                new NumberingFormats(
                    //  
                    new NumberingFormat()                                                  // Custom number format # 164: especially for date-times
                    {
                        NumberFormatId = UInt32Value.FromUInt32(iExcelIndex++),
                        FormatCode = StringValue.FromString("dd/MMM/yyyy hh:mm:ss")
                    },
                    new NumberingFormat()                                                   // Custom number format # 165: especially for date times (with a blank time)
                    {
                        NumberFormatId = UInt32Value.FromUInt32(iExcelIndex++),
                        FormatCode = StringValue.FromString("dd/MMM/yyyy")
                    }
               ),
                new Fonts(
                    new Font(                                                               // Index 0 - The default font.
                        new FontSize() { Val = 8 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "#000000" } },
                        new FontName() { Val = "Arial" }),
                    new Font(                                                               // Index 1 - A 12px bold font, in white.
                        new Bold(),
                        new FontSize() { Val = 10 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "#000000" } },
                        new FontName() { Val = "Arial" }),
                    new Font(                                                               // Index 2 - An Italic font.
                        new Italic(),
                        new FontSize() { Val = 8 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "#000000" } },
                        new FontName() { Val = "Times New Roman" })
                ),
                new Fills(
                    new Fill(                                                           // Index 0 - The default fill.
                        new PatternFill() { PatternType = PatternValues.None }),
                    new Fill(
                    new PatternFill(
                            new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "FF4B9DF0" } }
                    )
                    { PatternType = PatternValues.Solid }),
                    new Fill(                                                           // Index 2 - The yellow fill.
                        new PatternFill(
                            new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "FF4B9DF0" } }
                        )
                        { PatternType = PatternValues.Solid }),
                    new Fill(                                                           // Index 3 - Dark-gray fill.
                        new PatternFill(
                            new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "FF4B9DF0" } }
                        )
                        { PatternType = PatternValues.Solid })
                ),
                new Borders(
                    new Border(                                                         // Index 0 - The default border.
                        new LeftBorder(),
                        new RightBorder(),
                        new TopBorder(),
                        new BottomBorder(),
                        new DiagonalBorder()),
                    new Border(                                                         // Index 1 - Applies a Left, Right, Top, Bottom border to a cell
                        new LeftBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new RightBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new TopBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new BottomBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new DiagonalBorder())
                ),
                new CellFormats(
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 0 },                         // Style # 0 - The default cell style.  If a cell does not have a style index applied it will use this style combination instead
                    new CellFormat() { NumberFormatId = 164 },                                         // Style # 1 - DateTimes
                    new CellFormat() { NumberFormatId = 165 },                                         // Style # 2 - Dates (with a blank time)
                    new CellFormat(new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center })
                    { FontId = 1, FillId = 3, BorderId = 0, ApplyFont = true, ApplyAlignment = true },       // Style # 3 - Header row 
                    new CellFormat() { NumberFormatId = 3 },                                           // Style # 4 - Number format: #,##0
                    new CellFormat() { NumberFormatId = 4 },                                           // Style # 5 - Number format: #,##0.00
                    new CellFormat() { FontId = 1, FillId = 0, BorderId = 0, ApplyFont = true },       // Style # 6 - Bold 
                    new CellFormat() { FontId = 2, FillId = 0, BorderId = 0, ApplyFont = true },       // Style # 7 - Italic
                    new CellFormat() { FontId = 2, FillId = 0, BorderId = 0, ApplyFont = true },       // Style # 8 - Times Roman
                    new CellFormat() { FontId = 0, FillId = 2, BorderId = 0, ApplyFill = true },       // Style # 9 - Yellow Fill
                    new CellFormat(                                                                    // Style # 10 - Alignment
                        new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center }
                    )
                    { FontId = 0, FillId = 0, BorderId = 0, ApplyAlignment = true },
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true }      // Style # 11 - Border
                )
            ); // return
        }

        private static void WriteDataTableToExcelWorksheet(DataTable dt, WorksheetPart worksheetPart, DefinedNames definedNamesCol)
        {
            OpenXmlWriter writer = OpenXmlWriter.Create(worksheetPart, Encoding.UTF8);
            writer.WriteStartElement(new Worksheet());

            //  To demonstrate how to set column-widths in Excel, here's how to set the width of all columns to our default of "25":
            UInt32 inx = 1;
            writer.WriteStartElement(new Columns());
            foreach (DataColumn dc in dt.Columns)
            {
                writer.WriteElement(new Column { Min = inx, Max = inx, CustomWidth = true, Width = DEFAULT_COLUMN_WIDTH });
                inx++;
            }
            writer.WriteEndElement();


            writer.WriteStartElement(new SheetData());

            string cellValue = "";
            string cellReference = "";

            //  Create a Header Row in our Excel file, containing one header for each Column of data in our DataTable.
            //
            //  We'll also create an array, showing which type each column of data is (Text or Numeric), so when we come to write the actual
            //  cells of data, we'll know if to write Text values or Numeric cell values.
            int numberOfColumns = dt.Columns.Count;
            bool[] IsIntegerColumn = new bool[numberOfColumns];
            bool[] IsFloatColumn = new bool[numberOfColumns];
            bool[] IsDateColumn = new bool[numberOfColumns];

            string[] excelColumnNames = new string[numberOfColumns];
            for (int n = 0; n < numberOfColumns; n++)
                excelColumnNames[n] = GetExcelColumnNameForWriting(n);

            //
            //  Create the Header row in our Excel Worksheet
            //  We'll set the row-height to 20px, and (using the "AppendHeaderTextCell" function) apply some formatting to the cells.
            //
            uint rowIndex = 1;

            writer.WriteStartElement(new Row { RowIndex = rowIndex, Height = 20, CustomHeight = true });
            for (int colInx = 0; colInx < numberOfColumns; colInx++)
            {
                DataColumn col = dt.Columns[colInx];
                AppendHeaderTextCell(excelColumnNames[colInx] + "1", col.ColumnName, writer);
                IsIntegerColumn[colInx] = (col.DataType.FullName.StartsWith("System.Int"));
                IsFloatColumn[colInx] = (col.DataType.FullName == "System.Decimal") || (col.DataType.FullName == "System.Double") || (col.DataType.FullName == "System.Single");
                IsDateColumn[colInx] = (col.DataType.FullName == "System.DateTime");

                //  Uncomment the following lines, for an example of how to create some Named Ranges in your Excel file
#if FALSE
                //  For each column of data in this worksheet, let's create a Named Range, showing where there are values in this column
                //       eg  "NamedRange_UserID"  = "Drivers!$A2:$A6"
                //           "NamedRange_Surname" = "Drivers!$B2:$B6"
                string columnHeader = col.ColumnName.Replace(" ", "_");
                string NamedRange = string.Format("{0}!${1}2:${2}{3}", worksheetName, excelColumnNames[colInx], excelColumnNames[colInx], dt.Rows.Count + 1);
                DefinedName definedName = new DefinedName() { 
                    Name = "NamedRange_" + columnHeader,
                    Text = NamedRange 
                };       
                definedNamesCol.Append(definedName);        
#endif
            }
            writer.WriteEndElement();   //  End of header "Row"

            //
            //  Now, step through each row of data in our DataTable...
            //
            double cellFloatValue = 0;
            CultureInfo ci = new CultureInfo("en-US");
            foreach (DataRow dr in dt.Rows)
            {
                // ...create a new row, and append a set of this row's data to it.
                ++rowIndex;

                writer.WriteStartElement(new Row { RowIndex = rowIndex });

                for (int colInx = 0; colInx < numberOfColumns; colInx++)
                {
                    cellValue = dr.ItemArray[colInx].ToString();
                    cellValue = ReplaceHexadecimalSymbols(cellValue);
                    cellReference = excelColumnNames[colInx] + rowIndex.ToString();

                    // Create cell with data
                    if (IsIntegerColumn[colInx] || IsFloatColumn[colInx])
                    {
                        //  For numeric cells without any decimal places.
                        //  If this numeric value is NULL, then don't write anything to the Excel file.
                        cellFloatValue = 0;
                        bool bIncludeDecimalPlaces = IsFloatColumn[colInx];
                        if (double.TryParse(cellValue, out cellFloatValue))
                        {
                            cellValue = cellFloatValue.ToString(ci);
                            AppendNumericCell(cellReference, cellValue, bIncludeDecimalPlaces, writer);
                        }
                    }
                    else if (IsDateColumn[colInx])
                    {
                        //  For date values, we save the value to Excel as a number, but need to set the cell's style to format
                        //  it as either a date or a date-time.
                        DateTime dateValue;
                        if (DateTime.TryParse(cellValue, out dateValue))
                        {
                            AppendDateCell(cellReference, dateValue, writer);
                        }
                        else
                        {
                            //  This should only happen if we have a DataColumn of type "DateTime", but this particular value is null/blank.
                            AppendTextCell(cellReference, cellValue, writer);
                        }
                    }
                    else
                    {
                        //  For text cells, just write the input data straight out to the Excel file.
                        AppendTextCell(cellReference, cellValue, writer);
                    }
                }
                writer.WriteEndElement(); //  End of Row
            }
            writer.WriteEndElement(); //  End of SheetData
            writer.WriteEndElement(); //  End of worksheet

            writer.Close();
        }

        private static void AppendHeaderTextCell(string cellReference, string cellStringValue, OpenXmlWriter writer)
        {
            //  Add a new "text" Cell to the first row in our Excel worksheet
            //  We set these cells to use "Style # 3", so they have a gray background color & white text.
            writer.WriteElement(new Cell
            {
                CellValue = new CellValue(cellStringValue),
                CellReference = cellReference,
                DataType = CellValues.String,
                StyleIndex = 3
            });
        }

        private static void AppendTextCell(string cellReference, string cellStringValue, OpenXmlWriter writer)
        {
            //  Add a new "text" Cell to our Row 

#if DATA_CONTAINS_FORMULAE
            //  If this item of data looks like a formula, let's store it in the Excel file as a formula rather than a string.
            if (cellStringValue.StartsWith("="))
            {
                AppendFormulaCell(cellReference, cellStringValue, writer);
                return;
            }
#endif

            //  Add a new Excel Cell to our Row 
            writer.WriteElement(new Cell
            {
                CellValue = new CellValue(cellStringValue),
                CellReference = cellReference,
                DataType = CellValues.String
            });
        }

        private static void AppendDateCell(string cellReference, DateTime dateTimeValue, OpenXmlWriter writer)
        {
            //  Add a new "datetime" Excel Cell to our Row.
            //
            //  If the "time" part of the DateTime is blank, we'll format the cells as "dd/MMM/yyyy", otherwise ""dd/MMM/yyyy hh:mm:ss"
            //  (Feel free to modify this logic if you wish.)
            //
            //  In our GenerateStyleSheet() function, we defined two custom styles, to make this work:
            //      Custom style#1 is a style containing our custom NumberingFormat # 164 (show each date as "dd/MMM/yyyy hh:mm:ss")
            //      Custom style#2 is a style containing our custom NumberingFormat # 165 (show each date as "dd/MMM/yyyy")
            //  
            //  So, if our time element is blank, we'll assign style 2, but if there IS a time part, we'll apply style 1.
            //
            string cellStringValue = dateTimeValue.ToOADate().ToString(CultureInfo.InvariantCulture);
            bool bHasBlankTime = (dateTimeValue.Date == dateTimeValue);

            writer.WriteElement(new Cell
            {
                CellValue = new CellValue(cellStringValue),
                CellReference = cellReference,
                StyleIndex = UInt32Value.FromUInt32(bHasBlankTime ? (uint)2 : (uint)1),
                DataType = CellValues.Number        //  Use this, rather than CellValues.Date
            });
        }

        private static void AppendNumericCell(string cellReference, string cellStringValue, bool bIncludeDecimalPlaces, OpenXmlWriter writer)
        {
            //  Add a new numeric Excel Cell to our Row.
            UInt32 cellStyle = (UInt32)(bIncludeDecimalPlaces ? 5 : 4);
            writer.WriteElement(new Cell
            {
                CellValue = new CellValue(cellStringValue),
                CellReference = cellReference,
                StyleIndex = cellStyle,                                 //  Style #4 formats with 0 decimal places, style #5 formats with 2 decimal places
                DataType = CellValues.Number
            });
        }

        private static string ReplaceHexadecimalSymbols(string txt)
        {
            //  I've often seen cases when a non-ASCII character will slip into the data you're trying to export, and this will cause an invalid Excel to be created.
            //  This function removes such characters.
            string r = "[\x00-\x08\x0B\x0C\x0E-\x1F]";
            return Regex.Replace(txt, r, "", RegexOptions.Compiled);
        }

        //  Convert a zero-based column index into an Excel column reference  (A, B, C.. Y, Z, AA, AB, AC... AY, AZ, BA, BB..)
        public static string GetExcelColumnNameForWriting(int columnIndex)
        {
            //  Convert a zero-based column index into an Excel column reference  (A, B, C.. Y, Z, AA, AB, AC... AY, AZ, BA, BB..)
            //  eg GetExcelColumnName(0) should return "A"
            //     GetExcelColumnName(1) should return "B"
            //     GetExcelColumnName(25) should return "Z"
            //     GetExcelColumnName(26) should return "AA"
            //     GetExcelColumnName(27) should return "AB"
            //     GetExcelColumnName(701) should return "ZZ"
            //     GetExcelColumnName(702) should return "AAA"
            //     GetExcelColumnName(1999) should return "BXX"
            //     ..etc..

            int firstInt = columnIndex / 676;
            int secondInt = (columnIndex % 676) / 26;
            if (secondInt == 0)
            {
                secondInt = 26;
                firstInt = firstInt - 1;
            }
            int thirdInt = (columnIndex % 26);

            char firstChar = (char)('A' + firstInt - 1);
            char secondChar = (char)('A' + secondInt - 1);
            char thirdChar = (char)('A' + thirdInt);

            if (columnIndex < 26)
                return thirdChar.ToString();

            if (columnIndex < 702)
                return string.Format("{0}{1}", secondChar, thirdChar);

            return string.Format("{0}{1}{2}", firstChar, secondChar, thirdChar);
        }
    }
}
