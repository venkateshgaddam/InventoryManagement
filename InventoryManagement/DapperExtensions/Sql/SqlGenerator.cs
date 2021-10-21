using DapperExtensions.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DapperExtensions.Sql
{
    public interface ISqlGenerator
    {
        IDapperExtensionsConfiguration Configuration { get; }

        string Select(IClassMapper classMap, IPredicate predicate, IList<ISort> sort, IDictionary<string, object> parameters, string schema, IList<string> selectPropertyName = null, bool distinct = false);
        string SelectPaged(IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int page, int resultsPerPage, IDictionary<string, object> parameters, string schema, IList<string> selectPropertyName = null, bool distinct = false);
        string SelectSet(IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int firstResult, int maxResults, IDictionary<string, object> parameters, string schema, IList<string> selectPropertyName = null);
        string Count(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters, string schema);

        string Insert(IClassMapper classMap, string schema);
        string Update(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters, bool ignoreAllKeyProperties, string schema);
        string Delete(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters, string schema);

        string IdentitySql(IClassMapper classMap, string schema);
        string GetTableName(IClassMapper map, string schema);
        string GetColumnName(IClassMapper map, IPropertyMap property, bool includeAlias, string schema);
        string GetColumnName(IClassMapper map, string propertyName, bool includeAlias, string schema);
        bool SupportsMultipleStatements();

        #region CCP
        string InsertAndReturn(IClassMapper classMap, IPredicate existsPredicate, IDictionary<string, object> existsParameters, string schema);
        string UpdateAndReturn(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters, IPredicate existsPredicate, IDictionary<string, object> existsParameters, string schema, IEnumerable<string> cols = null);
        string DeleteSelected(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters, string schema);
        #endregion CCP
    }

    public class SqlGeneratorImpl : ISqlGenerator
    {
        public SqlGeneratorImpl(IDapperExtensionsConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IDapperExtensionsConfiguration Configuration { get; private set; }

        public virtual string Select(IClassMapper classMap, IPredicate predicate, IList<ISort> sort, IDictionary<string, object> parameters, string schema, IList<string> selectPropertyName, bool distinct = false)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("Parameters");
            }

            StringBuilder sql = new StringBuilder(string.Format("SELECT {0} {1} FROM {2}",
                (distinct ? "Distinct" : string.Empty),
                BuildSelectColumns(classMap, schema, selectPropertyName),
                GetTableName(classMap, schema)));

            if (predicate != null)
            {
                sql.Append(" WHERE ")
                    .Append(predicate.GetSql(this, parameters, schema));
            }

            if (sort != null && sort.Any())
            {
                sql.Append(" ORDER BY ")
                    .Append(sort.Select(s => GetColumnName(classMap, s.PropertyName, false, schema) + (s.Ascending ? " ASC" : " DESC")).AppendStrings());
            }

            return sql.ToString();
        }

        public virtual string SelectPaged(IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int page, int resultsPerPage, IDictionary<string, object> parameters, string schema, IList<string> selectPropertyName, bool distinct = false)
        {
            //if (sort == null || !sort.Any())
            //{
            //    throw new ArgumentNullException("Sort", "Sort cannot be null or empty.");
            //}

            if (parameters == null)
            {
                throw new ArgumentNullException("Parameters");
            }

            StringBuilder innerSql = new StringBuilder(string.Format("SELECT {0} {1} FROM {2}",
                (distinct ? "Distinct" : string.Empty),
                BuildSelectColumns(classMap, schema, selectPropertyName),
                GetTableName(classMap, schema)));

            if (predicate != null)
            {
                innerSql.Append(" WHERE ")
                    .Append(predicate.GetSql(this, parameters, schema));
            }

            string orderBy = sort?.Select(s => GetSimpleColumnName(classMap, s.PropertyName) + (s.Ascending ? " ASC" : " DESC"))?.AppendStrings();
            if (!string.IsNullOrEmpty(orderBy))
            {
                orderBy = " ORDER BY " + orderBy;
            }

            return Configuration.Dialect.GetOptimizedPagingSql(innerSql.ToString(), page, resultsPerPage, parameters, orderBy);
        }

        public virtual string SelectSet(IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int firstResult, int maxResults, IDictionary<string, object> parameters, string schema, IList<string> selectPropertyName)
        {
            if (sort == null || !sort.Any())
            {
                throw new ArgumentNullException("Sort", "Sort cannot be null or empty.");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("Parameters");
            }

            StringBuilder innerSql = new StringBuilder(string.Format("SELECT {0} FROM {1}",
                BuildSelectColumns(classMap, schema, selectPropertyName),
                GetTableName(classMap, schema)));
            if (predicate != null)
            {
                innerSql.Append(" WHERE ")
                    .Append(predicate.GetSql(this, parameters, schema));
            }

            string orderBy = sort.Select(s => GetColumnName(classMap, s.PropertyName, false, schema) + (s.Ascending ? " ASC" : " DESC")).AppendStrings();
            innerSql.Append(" ORDER BY " + orderBy);

            string sql = Configuration.Dialect.GetSetSql(innerSql.ToString(), firstResult, maxResults, parameters);
            return sql;
        }


        public virtual string Count(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters, string schema)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("Parameters");
            }

            StringBuilder sql = new StringBuilder(string.Format("SELECT COUNT(*) AS {0}Total{1} FROM {2}",
                                Configuration.Dialect.OpenQuote,
                                Configuration.Dialect.CloseQuote,
                                GetTableName(classMap, schema)));
            if (predicate != null)
            {
                sql.Append(" WHERE ")
                    .Append(predicate.GetSql(this, parameters, schema));
            }

            return sql.ToString();
        }

        public virtual string Insert(IClassMapper classMap, string schema)
        {
            var columns = classMap.Properties.Where(p => !(p.Ignored || p.IsReadOnly || p.KeyType == KeyType.Identity || p.KeyType == KeyType.TriggerIdentity));
            if (!columns.Any())
            {
                throw new ArgumentException("No columns were mapped.");
            }

            var columnNames = columns.Select(p => GetColumnName(classMap, p, false, schema));
            var parameters = columns.Select(p => Configuration.Dialect.ParameterPrefix + p.Name);

            string sql = string.Format("INSERT INTO {0} ({1}) VALUES ({2})",
                                       GetTableName(classMap, schema),
                                       columnNames.AppendStrings(),
                                       parameters.AppendStrings());

            var triggerIdentityColumn = classMap.Properties.Where(p => p.KeyType == KeyType.TriggerIdentity).ToList();

            if (triggerIdentityColumn.Count > 0)
            {
                if (triggerIdentityColumn.Count > 1)
                    throw new ArgumentException("TriggerIdentity generator cannot be used with multi-column keys");

                sql += string.Format(" RETURNING {0} INTO {1}IdOutParam", triggerIdentityColumn.Select(p => GetColumnName(classMap, p, false, schema)).First(), Configuration.Dialect.ParameterPrefix);
            }

            return sql;
        }

        public virtual string Update(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters, bool ignoreAllKeyProperties, string schema)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("Predicate");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("Parameters");
            }

            var columns = ignoreAllKeyProperties
                ? classMap.Properties.Where(p => !(p.Ignored || p.IsReadOnly) && p.KeyType == KeyType.NotAKey)
                : classMap.Properties.Where(p => !(p.Ignored || p.IsReadOnly || p.KeyType == KeyType.Identity || p.KeyType == KeyType.Assigned));

            if (!columns.Any())
            {
                throw new ArgumentException("No columns were mapped.");
            }

            var setSql =
                columns.Select(
                    p =>
                    string.Format(
                        "{0} = {1}{2}", GetColumnName(classMap, p, false, schema), Configuration.Dialect.ParameterPrefix, p.Name));

            return string.Format("UPDATE {0} SET {1} WHERE {2}",
                GetTableName(classMap, schema),
                setSql.AppendStrings(),
                predicate.GetSql(this, parameters, schema));
        }

        public virtual string Delete(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters, string schema)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("Predicate");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("Parameters");
            }

            StringBuilder sql = new StringBuilder(string.Format("DELETE FROM {0}", GetTableName(classMap, schema)));
            sql.Append(" WHERE ").Append(predicate.GetSql(this, parameters, schema));
            return sql.ToString();
        }

        public virtual string IdentitySql(IClassMapper classMap, string schema)
        {
            return Configuration.Dialect.GetIdentitySql(GetTableName(classMap, schema));
        }

        public virtual string GetTableName(IClassMapper map, string schema)
        {
            if (string.IsNullOrEmpty(schema))
                return Configuration.Dialect.GetTableName(map.SchemaName, map.TableName, null);
            else
                return Configuration.Dialect.GetTableName(schema, map.TableName, null);
        }

        public virtual string GetColumnName(IClassMapper map, IPropertyMap property, bool includeAlias, string schema)
        {
            string alias = null;
            if (property.ColumnName != property.Name && includeAlias)
            {
                alias = property.Name;
            }

            return Configuration.Dialect.GetColumnName(GetTableName(map, schema), property.ColumnName, alias);
        }

        public virtual string GetColumnName(IClassMapper map, string propertyName, bool includeAlias, string schema)
        {
            IPropertyMap propertyMap = map.Properties.SingleOrDefault(p => p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
            if (propertyMap == null)
            {
                throw new ArgumentException(string.Format("Could not find '{0}' in Mapping.", propertyName));
            }

            return GetColumnName(map, propertyMap, includeAlias, schema);
        }

        public virtual bool SupportsMultipleStatements()
        {
            return Configuration.Dialect.SupportsMultipleStatements;
        }

        public virtual string BuildSelectColumns(IClassMapper classMap, string schema, IList<string> selectPropertyName)
        {
            var columns = classMap.Properties
                .Where(p => !p.Ignored && !(selectPropertyName != null && !selectPropertyName.Contains(p.PropertyInfo.Name)))
                .Select(p => GetColumnName(classMap, p, true, schema));
            return columns.AppendStrings();
        }

        #region CCP
        private string GetSimpleColumnName(IClassMapper map, string propertyName)
        {
            IPropertyMap propertyMap = map.Properties.SingleOrDefault(p => p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
            if (propertyMap == null)
            {
                throw new ArgumentException(string.Format("Could not find '{0}' in Mapping.", propertyName));
            }

            return Configuration.Dialect.GetColumnName(null, propertyMap.ColumnName, null);
        }

        public virtual string InsertAndReturn(IClassMapper classMap, IPredicate existsPredicate, IDictionary<string, object> existsParameters, string schema)
        {
            var columns = classMap.Properties.Where(p => !(p.Ignored || p.IsReadOnly || p.KeyType == KeyType.Identity));
            if (!columns.Any())
            {
                throw new ArgumentException("No columns were mapped.");
            }

            var columnNames = columns.Select(p => GetColumnName(classMap, p, false, schema));
            var parameters = columns.Select(p => Configuration.Dialect.ParameterPrefix + p.Name);

            return Configuration.Dialect.GetInsertAndReturnSql(GetTableName(classMap, schema), 
                columnNames.AppendStrings(), parameters.AppendStrings(),
                existsPredicate?.GetSql(this, existsParameters, schema, useSuffix: false)); 
        }


        public virtual string UpdateAndReturn(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters, IPredicate existsPredicate, IDictionary<string, object> existsParameters, string schema, IEnumerable<string> cols = null)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("Predicate");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("Parameters");
            }

            IEnumerable<IPropertyMap> columns = null;

            if (cols == null)
            {
                columns = classMap.Properties.Where(p => !(p.Ignored || p.IsReadOnly || p.KeyType == KeyType.Identity));
            }
            else
            {
                columns = cols?.Join(classMap.Properties, x => x.ToLowerInvariant(), y => y.ColumnName.ToLowerInvariant(), (x, y) => y)?.Where(p => !(p.Ignored || p.IsReadOnly || p.KeyType == KeyType.Identity));
            }

            if (!columns?.Any() ?? true)
            {
                throw new ArgumentException("No columns were mapped.");
            }

            var setSql =
                columns.Select(
                    p =>
                    string.Format(
                        "{0} = {1}{2}", GetColumnName(classMap, p, false, schema), Configuration.Dialect.ParameterPrefix, p.Name));

            return Configuration.Dialect.GetUpdateAndReturn(GetTableName(classMap, schema),
                setSql.AppendStrings(), predicate.GetSql(this, parameters, schema),
                existsPredicate?.GetSql(this, existsParameters, schema, useSuffix: false));
        }

        public virtual string DeleteSelected(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters, string schema)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("Predicate");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("Parameters");
            }

            return Configuration.Dialect.GetDeleteSelected(GetTableName(classMap, schema), predicate.GetSql(this, parameters, schema));
        }
        #endregion CCP
    }
}