using System;
using System.Collections.Generic;

namespace DapperExtensions.Sql
{
    public class MySqlDialect : SqlDialectBase
    {
        public override char OpenQuote
        {
            get { return '`'; }
        }

        public override char CloseQuote
        {
            get { return '`'; }
        }

        public override string GetIdentitySql(string tableName)
        {
            return "SELECT CONVERT(LAST_INSERT_ID(), SIGNED INTEGER) AS ID";
        }

        public override string GetPagingSql(string sql, int page, int resultsPerPage, IDictionary<string, object> parameters)
        {            
            int startValue = page * resultsPerPage;
            return GetSetSql(sql, startValue, resultsPerPage, parameters);
        }

        public override string GetSetSql(string sql, int firstResult, int maxResults, IDictionary<string, object> parameters)
        {
            string result = string.Format("{0} LIMIT @firstResult, @maxResults", sql);
            parameters.Add("@firstResult", firstResult);
            parameters.Add("@maxResults", maxResults);
            return result;
        }

        #region CCP
        public override string ParameterPrefixFn
        {
            get { return "@"; }
        }

        public override string GetColumnNameFn(string columnName)
        {
            throw new NotImplementedException();
        }
        public override string GetInsertAndReturnSql(string tableName, string columnNames, string valueString, string existSql)
        {
            throw new NotImplementedException();
        }
        public override string GetUpdateAndReturn(string tableName, string setString, string conditionString, string existSql)
        {
            throw new NotImplementedException();
        }
        public override string GetDeleteSelected(string tableName, string conditionString)
        {
            throw new NotImplementedException();
        }
        public override string GetOptimizedPagingSql(string sql, int page, int resultsPerPage, IDictionary<string, object> parameters, string orderBy)
        {
            throw new NotImplementedException();
        }

        public override string GetLikeOperator(bool ignoreCase)
        {
            throw new NotImplementedException();
        }
        #endregion CCP
    }
}