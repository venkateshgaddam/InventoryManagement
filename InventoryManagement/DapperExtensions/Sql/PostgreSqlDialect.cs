using System;
using System.Collections.Generic;

namespace DapperExtensions.Sql
{
    public class PostgreSqlDialect : SqlDialectBase
    {
        public override string GetIdentitySql(string tableName)
        {
            return "SELECT LASTVAL() AS Id";
        }

        public override string GetPagingSql(string sql, int page, int resultsPerPage, IDictionary<string, object> parameters)
        {
            int startValue = page * resultsPerPage;
            return GetSetSql(sql, startValue, resultsPerPage, parameters);
        }

        public override string GetSetSql(string sql, int pageNumber, int maxResults, IDictionary<string, object> parameters)
        {
            string result = string.Format("{0} LIMIT @maxResults OFFSET @pageStartRowNbr", sql);
            parameters.Add("@maxResults", maxResults);
            parameters.Add("@pageStartRowNbr", pageNumber * maxResults);
            return result;
        }

        public override string GetColumnName(string prefix, string columnName, string alias)
        {
            return base.GetColumnName(null, columnName, alias).ToLower();
        }

        public override string GetTableName(string schemaName, string tableName, string alias)
        {
            return base.GetTableName(schemaName, tableName, alias).ToLower();
        }

        #region CCP
        public override string ParameterPrefixFn
        {
            get { return "p_"; }
        }

        public override string GetColumnNameFn(string columnName)
        {
            return (this.ParameterPrefixFn + columnName).ToLower();
        }
        public override string GetInsertAndReturnSql(string tableName, string columnNames, string valueString, string existSql)
        {
            if (!string.IsNullOrEmpty(existSql))
            {
                string sql = string.Format("INSERT INTO {0} ({1}) SELECT {2} ",
                                         tableName, columnNames, valueString);
                return $"{sql} WHERE ({existSql}) RETURNING *";
            }
            else
            {
                return string.Format("INSERT INTO {0} ({1}) VALUES ({2}) RETURNING *;",
                                       tableName, columnNames, valueString);
            }
        }
        public override string GetUpdateAndReturn(string tableName, string setString, string conditionString, string existSql)
        {
            if (!string.IsNullOrEmpty(existSql))
            {
                return string.Format("UPDATE {0} SET {1} WHERE {2} AND ({3}) RETURNING *;",
             tableName, setString, conditionString, existSql);
            }
            else
            {
                return string.Format("UPDATE {0} SET {1} WHERE {2} RETURNING *;",
                tableName, setString, conditionString);
            }
        }
        public override string GetDeleteSelected(string tableName, string conditionString)
        {
            return string.Format("DELETE FROM  {0}  WHERE {1} RETURNING *;", tableName, conditionString);
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
                                          ), Count_CTE AS (
                                        SELECT COUNT(0) as totalcount
                                        FROM Main_CTE
                                        )
                                        SELECT * from Count_CTE, Main_CTE
                                        {1} 
                                        LIMIT @pagesize OFFSET (@pagestartrow) * @pagesize;",
                                        sql, orderBy ?? string.Empty);

            parameters.Add("@pagestartrow", page);
            parameters.Add("@pagesize", resultsPerPage);
            return result;
        }

        public override string GetLikeOperator(bool ignoreCase)
        {
            if (ignoreCase)
            {
                return "ILIKE";
            }
            else
            {
                return "LIKE";
            }
        }

        #endregion CCP
    }
}
