using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DapperExtensions.Sql
{
    public class SqlServerDialect : SqlDialectBase
    {
        public override char OpenQuote
        {
            get { return '['; }
        }

        public override char CloseQuote
        {
            get { return ']'; }
        }

        public override string GetIdentitySql(string tableName)
        {
            return string.Format("SELECT CAST(SCOPE_IDENTITY()  AS BIGINT) AS [Id]");
        }

        public override string GetPagingSql(string sql, int page, int resultsPerPage, IDictionary<string, object> parameters)
        {
            int startValue = (page * resultsPerPage) + 1;
            return GetSetSql(sql, startValue, resultsPerPage, parameters);
        }

        public override string GetSetSql(string sql, int firstResult, int maxResults, IDictionary<string, object> parameters)
        {
            if (string.IsNullOrEmpty(sql))
            {
                throw new ArgumentNullException("SQL");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("Parameters");
            }

            int selectIndex = GetSelectEnd(sql) + 1;
            string orderByClause = GetOrderByClause(sql);
            if (orderByClause == null)
            {
                orderByClause = "ORDER BY CURRENT_TIMESTAMP";
            }


            string projectedColumns = GetColumnNames(sql).Aggregate(new StringBuilder(), (sb, s) => (sb.Length == 0 ? sb : sb.Append(", ")).Append(GetColumnName("_proj", s, null)), sb => sb.ToString());
            string newSql = sql
                .Replace(" " + orderByClause, string.Empty)
                .Insert(selectIndex, string.Format("ROW_NUMBER() OVER(ORDER BY {0}) AS {1}, ", orderByClause.Substring(9), GetColumnName(null, "_row_number", null)));

            string result = string.Format("SELECT TOP({0}) {1} FROM ({2}) [_proj] WHERE {3} >= @_pageStartRow ORDER BY {3}",
                maxResults, projectedColumns.Trim(), newSql, GetColumnName("_proj", "_row_number", null));

            parameters.Add("@_pageStartRow", firstResult);
            return result;
        }

        protected string GetOrderByClause(string sql)
        {
            int orderByIndex = sql.LastIndexOf(" ORDER BY ", StringComparison.InvariantCultureIgnoreCase);
            if (orderByIndex == -1)
            {
                return null;
            }

            string result = sql.Substring(orderByIndex).Trim();

            int whereIndex = result.IndexOf(" WHERE ", StringComparison.InvariantCultureIgnoreCase);
            if (whereIndex == -1)
            {
                return result;
            }

            return result.Substring(0, whereIndex).Trim();
        }

        protected int GetFromStart(string sql)
        {
            int selectCount = 0;
            string[] words = sql.Split(' ');
            int fromIndex = 0;
            foreach (var word in words)
            {
                if (word.Equals("SELECT", StringComparison.InvariantCultureIgnoreCase))
                {
                    selectCount++;
                }

                if (word.Equals("FROM", StringComparison.InvariantCultureIgnoreCase))
                {
                    selectCount--;
                    if (selectCount == 0)
                    {
                        break;
                    }
                }

                fromIndex += word.Length + 1;
            }

            return fromIndex;
        }

        protected virtual int GetSelectEnd(string sql)
        {
            if (sql.StartsWith("SELECT DISTINCT", StringComparison.InvariantCultureIgnoreCase))
            {
                return 15;
            }

            if (sql.StartsWith("SELECT", StringComparison.InvariantCultureIgnoreCase))
            {
                return 6;
            }

            throw new ArgumentException("SQL must be a SELECT statement.", "sql");
        }

        protected virtual IList<string> GetColumnNames(string sql)
        {
            int start = GetSelectEnd(sql);
            int stop = GetFromStart(sql);
            string[] columnSql = sql.Substring(start, stop - start).Split(',');
            List<string> result = new List<string>();
            foreach (string c in columnSql)
            {
                int index = c.IndexOf(" AS ", StringComparison.InvariantCultureIgnoreCase);
                if (index > 0)
                {
                    result.Add(c.Substring(index + 4).Trim());
                    continue;
                }

                string[] colParts = c.Split('.');
                result.Add(colParts[colParts.Length - 1].Trim());
            }

            return result;
        }

        #region CCP
        public override string ParameterPrefixFn
        {
            get { return "@"; }
        }

        public override string GetColumnNameFn(string columnName)
        {
            return this.ParameterPrefixFn + columnName;
        }

        public override string GetInsertAndReturnSql(string tableName, string columnNames, string valueString, string existSql)
        {
            string sql = string.Format("INSERT INTO {0} ({1}) OUTPUT INSERTED.* VALUES ({2})",
                                       tableName, columnNames, valueString);

            if (!string.IsNullOrEmpty(existSql))
            {
                sql = $"IF {existSql} BEGIN {sql} END;";
            }
            return sql;
        }
        public override string GetUpdateAndReturn(string tableName, string setString, string conditionString, string existSql)
        {
            var sql = string.Format("UPDATE {0} SET {1} OUTPUT INSERTED.* WHERE {2}",
                tableName, setString, conditionString);

            if (!string.IsNullOrEmpty(existSql))
            {
                sql = $"IF {existSql} BEGIN {sql} END;";
            }
            return sql;
        }
        public override string GetDeleteSelected(string tableName, string conditionString)
        {
            return string.Format("DELETE FROM {0} OUTPUT DELETED.* WHERE {1} ", tableName, conditionString);
        }
        public override string GetOptimizedPagingSql(string sql, int page, int resultsPerPage, IDictionary<string, object> parameters, string orderBy)
        {
            if (string.IsNullOrEmpty(sql))
            {
                throw new ArgumentNullException("SQL");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("Parameters");
            }

            if (string.IsNullOrEmpty(orderBy))
            {
                orderBy = "ORDER BY CURRENT_TIMESTAMP";
            }

            string result = string.Format(@";WITH Main_CTE AS
                                          (
                                            {0}
                                          )
                                           , Count_CTE AS (
                                        SELECT COUNT(0) as [TotalCount]
                                        FROM Main_CTE
                                        )
                                        SELECT * from Count_CTE, Main_CTE
                                        {1} 
                                        OFFSET (@_pageStartRow) * @PageSize ROWS
                                        FETCH NEXT @PageSize ROWS ONLY",

                sql, orderBy ?? string.Empty);

            parameters.Add("@_pageStartRow", page);
            parameters.Add("@PageSize", resultsPerPage);
            return result;
        }

        public override string GetLikeOperator(bool ignoreCase)
        {
            if (ignoreCase)
            {
                return "LIKE";
            }
            else
            {
                throw new NotSupportedException("Case sensitive operation is not allowed");
            }
        }

        #endregion CCP

    }
}