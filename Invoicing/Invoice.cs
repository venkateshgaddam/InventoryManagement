using System;
using System.Collections.Generic;
using System.Linq;

namespace Invoicing
{
    public class Invoice : Order
    {
        readonly List<Product> productsList;
        readonly List<string> VAT_exmpeted_Categories;
        double total_amount = 0;
        double Value_Added_Tax = 0;
        double import_taxes = 0;

        public Invoice(List<Product> products, List<string> vat_exemptions) : base(products)
        {
            productsList = products;
            VAT_exmpeted_Categories = vat_exemptions;
        }

        public override void PrintInvoice(string[] fileInput)
        {
            int products_count = 0;
            Console.Write($"Name");
            Console.Write("                                     ");
            Console.Write(" | QTY | UNIT_COST | COST");
            Console.WriteLine();

            foreach (var item in fileInput)
            {
                ++products_count;
                string productName = item.Split('@')[0][2..];
                string product_quantity = item.Split('@')[0].Split(' ')[0];
                double unitCost = Convert.ToDouble(item.Split('@')[1].TrimStart());
                double total_cost = Convert.ToDouble(item.Split('@')[0].Split(' ')[0]) * unitCost;
                total_amount += total_cost;
                Product product_details = GetProductDetails(productName);
                Value_Added_Tax += CalculateVAT(total_cost, product_details);
                import_taxes += CalculateImportTaxes(total_cost, product_details);
                Console.Write(productName);
                Console.Write("                                         ".Substring(productName.Length));
                Console.Write(" |  " + item.Split('@')[0].Split(' ')[0] + "  |   " + unitCost + "   |  " + total_cost);
                Console.WriteLine();
            }
            Console.WriteLine($"Total Items : {products_count}");
            Console.WriteLine($"SubTotal: {total_amount}");
            Console.WriteLine($"Value Added Tax: {Value_Added_Tax}");
            Console.WriteLine($"Additional Taxes: {import_taxes}");
            Console.WriteLine($"Total Amount: {import_taxes + Value_Added_Tax + total_amount}");
        }
        private double CalculateImportTaxes(double total_amount, Product product_details)
        {
            if (product_details != null && product_details.Isimported)
            {
                return Math.Round(total_amount * .024, 2);
            }
            return 0;
        }

        private double CalculateVAT(double total_amount, Product product_details)
        {
            if (product_details != null)
            {
                if (!VAT_exmpeted_Categories.Contains(product_details.producttype))
                {
                    return Math.Round(total_amount * .125, 2);
                }

            }
            return 0;
        }
    }
}
