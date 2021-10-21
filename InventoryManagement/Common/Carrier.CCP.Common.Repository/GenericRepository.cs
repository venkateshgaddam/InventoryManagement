using Carrier.CCP.Common.Database.Interface;
using Carrier.CCP.Common.Utils.Exception;
using Carrier.logging.Service;
using Dapper;
using DapperExtensions;
using DapperExtensions.Mapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Npgsql;
using Polly;
using Polly.Registry;
using Polly.Wrap;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Carrier.CCP.Common.Repository.Sql
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Carrier.CCP.Common.Repository.Sql.IGenericRepository{T}" />
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly IDbConnection _dbConnection;
        private readonly AsyncPolicyWrap _asyncPolicyWrap;
        private readonly PolicyWrap _policyWrap;
        private readonly string _entityName;
        private bool _disposed;
        private readonly Context _context;
        private readonly bool _isPostgreDbEngine = false;
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepository{T}"/> class.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <param name="registry">The registry.</param>
        /// <param name="logger"> The logger </param>
        /// <exception cref="ArgumentNullException">conn - The parameter {nameof(conn)} can't be null</exception>
        public GenericRepository(IDbConnection conn, IReadOnlyPolicyRegistry<string> registry, IPlatformLogger logger)
        {
            _context = new Context($"CallBack-{Guid.NewGuid()}", new Dictionary<string, object> { { "logger", logger } });
            _dbConnection = conn ?? throw new ArgumentNullException(nameof(conn), $"The parameter {nameof(conn)} can't be null");
            _entityName = typeof(T).Name;
            _asyncPolicyWrap = registry?.Get<AsyncPolicyWrap>("SQLAsyncResilienceStrategy") ?? Polly.Policy.WrapAsync(Polly.Policy.NoOpAsync());
            _policyWrap = registry?.Get<PolicyWrap>("SQLResilienceStrategy") ?? Polly.Policy.Wrap(Polly.Policy.NoOp());
            _isPostgreDbEngine = DapperAsyncExtensions.SqlDialect.GetType().Name.Equals("PostgreSqlDialect", StringComparison.InvariantCultureIgnoreCase);
        }

        #region Dispose

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing && _dbConnection != null && _dbConnection.State != ConnectionState.Closed)
                    _dbConnection.Close();
                _disposed = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Query Based Operation

        //public async Task<int> CountAsync(string schema = null)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Gets all asynchronous.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetAllAsync(string schema)
        {
            string sqlQuery = $"SELECT * FROM {DapperAsyncExtensions.SqlDialect.GetTableName(schema, _entityName, null)}";
            return await PollyExecuteAsync<IEnumerable<T>>(async () => await _dbConnection.QueryAsync<T>(sqlQuery)).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public virtual async Task<T> GetAsync(object name, dynamic id, string schema)
        {
            string tableName = DapperAsyncExtensions.SqlDialect.GetTableName(schema, _entityName, null);
            string sqlQuery = $"SELECT * FROM {tableName} WHERE {DapperAsyncExtensions.SqlDialect.GetColumnName(tableName, Convert.ToString(name), null)} = @Id";
            return await PollyExecuteAsync<T>(async () => await _dbConnection.QuerySingleOrDefaultAsync<T>(sqlQuery, new { Id = id })).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="filters">The filters.</param>
        /// <returns></returns>
        public virtual async Task<T> GetAsync(string schema = null, IFilter filters = null)
        {
            var predicateParam = PreparePredicate(filters);
            return await PollyExecuteAsync<T>(async () => await _dbConnection.GetRecordAsync<T>(predicateParam, schema: schema)).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="selectPropertyName"></param>
        /// <returns></returns>
        public virtual async Task<T> GetAsync(string schema = null, IFilter filters = null, IList<string> selectPropertyName = null)
        {
            var predicateParam = PreparePredicate(filters);
            return await PollyExecuteAsync<T>(async () => await _dbConnection.GetRecordAsync<T>(predicateParam, schema: schema, selectPropertyName: selectPropertyName)).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public T Get(object name, dynamic id, string schema)
        {
            string tableName = DapperAsyncExtensions.SqlDialect.GetTableName(schema, _entityName, null);
            string sqlQuery = $"SELECT * FROM {tableName} WHERE {DapperAsyncExtensions.SqlDialect.GetColumnName(tableName, Convert.ToString(name), null)} = @Id";
            return PollyExecute<T>(() => _dbConnection.QuerySingleOrDefault<T>(sqlQuery, new { Id = id }));
        }

        /// <summary>
        /// Gets the list asynchronous.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="sort">The sort.</param>
        /// <param name="selectPropertyName">Name of the select property.</param>
        /// <param name="distinct">if set to <c>true</c> [distinct].</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetListAsync(string schema = null, IFilter filters = null, IList<IOrder> sort = null,
            IList<string> selectPropertyName = null, bool distinct = false)
        {
            var predicateParam = PreparePredicate(filters);
            return await PollyExecuteAsync<IEnumerable<T>>(async () => await _dbConnection.GetListAsync<T>(predicateParam, AddSortCondition(sort),
                schema: schema, selectPropertyName: selectPropertyName, distinct: distinct)).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the paged asynchronous.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="sort">The sort.</param>
        /// <param name="selectPropertyName">Name of the select property.</param>
        /// <param name="distinct">if set to <c>true</c> [distinct].</param>
        /// <returns></returns>
        public virtual async Task<(IEnumerable<T>, long)> GetPagedAsync(int limit, int offset, string schema = null, IFilter filters = null,
            IList<IOrder> sort = null, IList<string> selectPropertyName = null, bool distinct = false)
        {
            var predicateParam = PreparePredicate(filters);
            return await PollyExecuteAsync<T>(async () => await _dbConnection.GetPageAsync<T>(predicateParam, AddSortCondition(sort),
                schema: schema, page: offset, resultsPerPage: limit, selectPropertyName: selectPropertyName, distinct: distinct)).ConfigureAwait(false);
        }

        #endregion Query Based Operation

        #region Command Based Operation

        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="existFilter">The Exist Filters.</param>
        /// <returns></returns>
        public virtual async Task<T> AddAsync(T entity, string schema, IFilter existFilter = null)
        {
            var existsPredicate = PreparePredicate(existFilter);
            return await PollyExecuteAsync<T>(async () => await _dbConnection.InsertEntityAsync(entity, schema: schema, existsPredicate: existsPredicate)).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="filters">the filters </param>
        /// <param name = "exitsFilter" > Used to check for precondition before insertion</param>
        /// <returns></returns>
        public virtual async Task<T> UpdateAsync(T entity, string schema, IFilter filters = null, IFilter exitsFilter = null)
        {
            var predicateParam = PreparePredicate(filters);
            var existsPredicateParam = PreparePredicate(exitsFilter);
            return await PollyExecuteAsync<T>(async () => await _dbConnection.UpdateAsync(entity, predicate: predicateParam,
                existsPredicate: existsPredicateParam, schema: schema)).ConfigureAwait(false);
        }


        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="cols">The list of columns.</param>
        /// <param name="exitsFilter">Used to check for precondition before insertion</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> UpdateAsync(T entity, string schema, IFilter filters, IEnumerable<string> cols, IFilter exitsFilter = null)
        {
            var predicateParam = PreparePredicate(filters);
            var existsPredicateParam = PreparePredicate(exitsFilter);
            return await PollyExecuteAsync<IEnumerable<T>>(async () => await _dbConnection.UpdateMultipleAsync(entity, predicateParam, cols,
                existsPredicateParam, schema: schema)).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public virtual async Task<int> DeleteAsync(object name, dynamic id, string schema)
        {
            string tableName = DapperAsyncExtensions.SqlDialect.GetTableName(schema, _entityName, null);
            string sqlQuery = $"DELETE FROM {tableName} WHERE {DapperAsyncExtensions.SqlDialect.GetColumnName(tableName, Convert.ToString(name), null)} = @Id";
            return await PollyExecuteAsync<int>(async () => await _dbConnection.ExecuteAsync(sqlQuery, new { Id = id })).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(T entity, string schema = null)
        {
            return await PollyExecuteAsync<int>(async () => await _dbConnection.DeleteByIdAsync(entity, schema: schema)).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="filters">The filters.</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> DeleteAsync(string schema, IFilter filters)
        {
            var predicateParam = PreparePredicate(filters);
            return await PollyExecuteAsync<IEnumerable<T>>(async () => await _dbConnection.DeleteSelectedAsync<T>(predicateParam, schema: schema)).ConfigureAwait(false);
        }

        #endregion Command Based Operation

        #region Stored Procedure API

        #region SP Query Operation

        private string FormatGetAllStoredProc(string spSchema, string storedProc)
        {
            return $"{DapperAsyncExtensions.SqlDialect.GetTableName(spSchema, (string.IsNullOrEmpty(storedProc) ? $"spGetAll{_entityName}" : $"{storedProc}"), null)}";
        }

        private string FormatGetByIdStoredProc(string spSchema, string storedProc)
        {
            return $"{DapperAsyncExtensions.SqlDialect.GetTableName(spSchema, (string.IsNullOrEmpty(storedProc) ? $"spGet{_entityName}ById" : $"{storedProc}"), null)}";
        }

        /// <summary>
        /// Sps the get all asynchronous.
        /// </summary>
        /// <param name="spSchema">The sp schema.</param>
        /// <param name="storedProc">The stored proc.</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> SP_GetListAsync(string spSchema = null, string storedProc = null)
        {
            var sqlQuery = FormatGetAllStoredProc(spSchema, storedProc);
            return await PollyExecuteAsync<IEnumerable<T>>(async () => await _dbConnection.QueryAsync<T>(sqlQuery,
                    commandType: CommandType.StoredProcedure)).ConfigureAwait(false);
        }

        /// <summary>
        /// Sps the get list by identifier asynchronous.
        /// </summary>
        /// <param name="dbInputs">The database inputs.</param>
        /// <param name="spSchema">The sp schema.</param>
        /// <param name="storedProc">The stored proc.</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> SP_GetListByFilterAsync(IDictionary<object, dynamic> dbInputs, string spSchema = null, string storedProc = null)
        {
            var parameters = new DynamicParameters();
            dbInputs.ToList().ForEach(a => parameters.Add($"@{a.Key}", a.Value));
            var sqlQuery = FormatGetAllStoredProc(spSchema, storedProc);

            return await PollyExecuteAsync<IEnumerable<T>>(async () => await _dbConnection.QueryAsync<T>(sqlQuery, parameters,
                    commandType: CommandType.StoredProcedure)).ConfigureAwait(false);
        }

        /// <summary>
        /// Sps the get all asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="spSchema">The sp schema.</param>
        /// <param name="storedProc">The stored proc.</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> SP_GetListByEntityAsync(T entity, string spSchema = null, string storedProc = null)
        {
            var parameters = new DynamicParameters();
            ExtractParameters(entity, parameters);
            var sqlQuery = FormatGetAllStoredProc(spSchema, storedProc);

            return await PollyExecuteAsync<IEnumerable<T>>(async () => await _dbConnection.QueryAsync<T>(sqlQuery, parameters,
                    commandType: CommandType.StoredProcedure)).ConfigureAwait(false);
        }

        /// <summary>
        /// Sps the get by identifier asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="spSchema">The sp schema.</param>
        /// <param name="storedProc">The stored proc.</param>
        /// <returns></returns>
        public virtual async Task<T> SP_GetByIdAsync(object name, dynamic id, string spSchema = null, string storedProc = null)
        {
            var sqlQuery = FormatGetByIdStoredProc(spSchema, storedProc);
            var parameters = new DynamicParameters();
            parameters.Add($"@{name}", id);

            return await PollyExecuteAsync<T>(async () => await _dbConnection.QuerySingleOrDefaultAsync<T>(sqlQuery, parameters,
                    commandType: CommandType.StoredProcedure)).ConfigureAwait(false);
        }

        /// <summary>
        /// Sps the get scalar by identifier asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="spSchema">The sp schema.</param>
        /// <param name="storedProc">The stored proc.</param>
        /// <returns></returns>
        public virtual async Task<dynamic> SP_GetColumnByIdAsync(object name, dynamic id, string spSchema, string storedProc)
        {
            var parameters = new DynamicParameters();
            parameters.Add($"@{name}", id);
            var sqlQuery = FormatGetByIdStoredProc(spSchema, storedProc);

            return await PollyExecuteAsync<dynamic>(async () => await _dbConnection.ExecuteScalarAsync<dynamic>(sqlQuery, parameters,
                    commandType: CommandType.StoredProcedure)).ConfigureAwait(false);
        }

        #endregion SP Query Operation

        #region SP Add

        /// <summary>
        /// Sps the add asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="storedProc">The stored proc.</param>
        /// <param name="spSchema">The sp schema.</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TResult>> SP_AddAsync<TResult>(T entity, string storedProc, string spSchema = null)
        {
            var parameters = new DynamicParameters();
            ExtractParameters(entity, parameters);
            string sqlQuery = $"{DapperAsyncExtensions.SqlDialect.GetTableName(spSchema, (string.IsNullOrEmpty(storedProc) ? $"spAdd{_entityName}" : $"{storedProc}"), null)}";

            return await PollyExecuteAsync<IEnumerable<TResult>>(async () => await _dbConnection.QueryAsync<TResult>(sqlQuery, parameters,
                     commandType: CommandType.StoredProcedure)).ConfigureAwait(false);
        }
        #endregion SP Add

        #region SP Update

        /// <summary>
        /// Formats the update stored proc.
        /// </summary>
        /// <param name="spSchema">The sp schema.</param>
        /// <param name="storedProc">The stored proc.</param>
        /// <returns></returns>
        private string FormatUpdateStoredProc(string spSchema, string storedProc)
        {
            return $"{DapperAsyncExtensions.SqlDialect.GetTableName(spSchema, (string.IsNullOrEmpty(storedProc) ? $"spUpdate{_entityName}" : $"{storedProc}"), null)}";
        }

        /// <summary>
        /// Sps the update asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="id">The identifier or Data Table.</param>
        /// <param name="spSchema">The sp schema.</param>
        /// <param name="storedProc">The stored proc.</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TResult>> SP_UpdateByTVPAsync<TResult>(object name, dynamic id, string spSchema = null, string storedProc = null)
        {
            var parameters = new DynamicParameters();            
            parameters.Add(DapperAsyncExtensions.SqlDialect.GetColumnNameFn(name.ToString()),
                id is DataTable ? !_isPostgreDbEngine ? (id as DataTable)?.AsTableValuedParameter()
                : ConvertDataTableToJson(id as DataTable) : id);
            var sqlQuery = FormatUpdateStoredProc(spSchema, storedProc);

            return await PollyExecuteAsync<IEnumerable<TResult>>(async () => await _dbConnection.QueryAsync<TResult>(sqlQuery,
                parameters, commandType: CommandType.StoredProcedure)).ConfigureAwait(false);
        }

        /// <summary>
        /// Sps the update asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="storedProc">The stored proc.</param>
        /// <param name="spSchema">The sp schema.</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TResult>> SP_UpdateByEntityAsync<TResult>(T entity, string spSchema = null, string storedProc = null)
        {
            ValidateIdentity(entity);
            var parameters = new DynamicParameters();
            ExtractParameters(entity, parameters);
            var sqlQuery = FormatUpdateStoredProc(spSchema, storedProc);

            return await PollyExecuteAsync<IEnumerable<TResult>>(async () => await _dbConnection.QueryAsync<TResult>(sqlQuery,
                parameters, commandType: CommandType.StoredProcedure)).ConfigureAwait(false);
        }

        #endregion SP Update

        #region SP Delete

        /// <summary>
        /// Formats the delete stored proc.
        /// </summary>
        /// <param name="spSchema">The sp schema.</param>
        /// <param name="storedProc">The stored proc.</param>
        /// <returns></returns>
        private string FormatDeleteStoredProc(string spSchema, string storedProc)
        {
            return $"{DapperAsyncExtensions.SqlDialect.GetTableName(spSchema, (string.IsNullOrEmpty(storedProc) ? $"spDelete{_entityName}" : $"{storedProc}"), null)}";
        }

        /// <summary>
        /// Sps the delete asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="id">The identifier or Data Table.</param>
        /// <param name="spSchema">The sp schema.</param>
        /// <param name="storedProc">The stored proc.</param>
        /// <returns></returns>
        public virtual async Task<int> SP_DeleteByTVPAsync(object name, dynamic id, string spSchema = null, string storedProc = null)
        {
            var parameters = new DynamicParameters();            
            parameters.Add(DapperAsyncExtensions.SqlDialect.GetColumnNameFn(name.ToString()),
               id is DataTable ? !_isPostgreDbEngine ? (id as DataTable)?.AsTableValuedParameter()
               : ConvertDataTableToJson(id as DataTable) : id);
            var sqlQuery = FormatDeleteStoredProc(spSchema, storedProc);

            return await PollyExecuteAsync<int>(async () => await _dbConnection.ExecuteAsync(sqlQuery,
                parameters, commandType: CommandType.StoredProcedure)).ConfigureAwait(false);
        }

        /// <summary>
        /// Sps the delete asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="spSchema">The sp schema.</param>
        /// <param name="storedProc">The stored proc.</param>
        /// <returns></returns>
        public virtual async Task<int> SP_DeleteByEntityAsync(T entity, string spSchema = null, string storedProc = null)
        {
            var parameters = new DynamicParameters();
            ExtractParameters(entity, parameters);
            var sqlQuery = FormatDeleteStoredProc(spSchema, storedProc);

            return await PollyExecuteAsync<int>(async () => await _dbConnection.ExecuteAsync(sqlQuery,
                parameters, commandType: CommandType.StoredProcedure)).ConfigureAwait(false);
        }

        #endregion SP Delete

        #endregion Stored Procedure API

        #region SP Helper Functions
        /// <summary>
        /// Validates the identity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <exception cref="ArgumentException">
        /// At least one Key column must be defined.
        /// or
        /// Key Value Must be not null.
        /// </exception>
        protected virtual void ValidateIdentity(T entity)
        {
            var sqlGenerator = _dbConnection.SqlQueryGenerator();
            var classMap = sqlGenerator.Configuration.GetMap<T>();
            var whereFields = classMap.Properties.Where(p => p.KeyType != KeyType.NotAKey);
            if (!whereFields.Any()) throw new ArgumentException("At least one Key column must be defined.");

            foreach (var column in whereFields)
            {
                if (IsGuidKeyType(column, entity) || IsIdentityKeyType(entity, column) || IsAssignedKeyType(entity, column))
                    throw new ArgumentException("Key Value Must be not null.");
            }
        }

        private bool IsAssignedKeyType(T entity, IPropertyMap column)
        {
            return column.KeyType == KeyType.Assigned && column.PropertyInfo.GetValue(entity, null) == null;
        }

        private bool IsIdentityKeyType(T entity, IPropertyMap column)
        {
            return column.KeyType == KeyType.Identity && (int)column.PropertyInfo.GetValue(entity, null) <= 0;
        }

        private bool IsGuidKeyType(IPropertyMap column, T entity)
        {
            return column.KeyType == KeyType.Guid && (Guid)column.PropertyInfo.GetValue(entity, null) == Guid.Empty;
        }

        /// <summary>
        /// Extracts the parameters.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="parameter">The parameter.</param>
        /// <exception cref="NotSupportedException">Trigger Identity is not supported.</exception>
        /// <exception cref="ArgumentException">No columns were mapped.</exception>
        protected virtual void ExtractParameters(T entity, DynamicParameters parameter)
        {
            var sqlGenerator = _dbConnection.SqlQueryGenerator();
            var classMap = sqlGenerator.Configuration.GetMap<T>();
            var nonIdentityKeyProperties = classMap.Properties
                .Where(p => p.KeyType == KeyType.Guid || p.KeyType == KeyType.Assigned).ToList();
            var identityColumn = classMap.Properties.SingleOrDefault(p => p.KeyType == KeyType.Identity);

            var triggerIdentityColumn = classMap.Properties.SingleOrDefault(p => p.KeyType == KeyType.TriggerIdentity);
            if (triggerIdentityColumn != null) throw new NotSupportedException("Trigger Identity is not supported.");

            var columns = classMap.Properties.Where(p => !(p.Ignored || p.IsReadOnly || p.KeyType == KeyType.Identity));
            if (!columns.Any()) throw new ArgumentException("No columns were mapped.");

            foreach (var column in nonIdentityKeyProperties)
                if (column.KeyType == KeyType.Guid && (Guid)column.PropertyInfo.GetValue(entity, null) == Guid.Empty)
                {
                    var comb = sqlGenerator.Configuration.GetNextGuid();
                    column.PropertyInfo.SetValue(entity, comb, null);
                }

            foreach (var column in columns)
            {
                var value = column.PropertyInfo.GetValue(entity, null);
                if (value is DataTable)
                    parameter.Add(DapperAsyncExtensions.SqlDialect.GetColumnNameFn(column.Name),
                        !_isPostgreDbEngine ? (value as DataTable)?.AsTableValuedParameter() : ConvertDataTableToJson(value as DataTable),
                direction: ParameterDirection.Input);
                else
                    parameter.Add(DapperAsyncExtensions.SqlDialect.GetColumnNameFn(column.Name), value, direction: ParameterDirection.Input);
            }
            //parameter.Add(DapperAsyncExtensions.SqlDialect.ParameterPrefixFn + "PK", direction: ParameterDirection.ReturnValue);
        }

        #endregion

        #region Convert IFilter to IPredicate

        private void ParseGroupPredicate(IFilter filters, PredicateGroup predicateGroups)
        {
            if (filters is IFilterGroup)
            {
                var fg = filters as IFilterGroup;
                foreach (var predicate in fg.Predicates)
                    if (predicate is IFilterGroup)
                    {
                        if (predicate is IFilterGroup fgchild)
                        {
                            var pg = new PredicateGroup
                            { Operator = (GroupOperator)(int)fgchild.Operator, Predicates = new List<IPredicate>() };
                            predicateGroups.Predicates.Add(pg);
                            ParseGroupPredicate(fgchild, pg);
                        }
                    }
                    else if (predicate is IFilterField<T>)
                    {
                        var filterField = predicate as IFilterField<T>;
                        predicateGroups.Predicates.Add(ParseFieldFilter(filterField));
                    }
            }
            else if (filters is IFilterField<T>)
            {
                predicateGroups.Predicates.Add(ParseFieldFilter(filters));
            }
        }

        private IList<ISort> AddSortCondition(IList<IOrder> sort)
        {
            IList<ISort> dapperSort = new List<ISort>();
            foreach (var sortItem in sort ?? new List<IOrder>())
                if (typeof(T).GetProperties()
                    .Where(x => x.Name?.ToLowerInvariant() == sortItem?.PropertyName?.ToLowerInvariant()).Any())
                    dapperSort.Add(new Sort { Ascending = sortItem.Ascending, PropertyName = sortItem.PropertyName });
                else
                    throw new ArgumentException($"Trying to sort on invalid column name ({sortItem?.PropertyName}).");

            return dapperSort;
        }

        private FieldPredicate<T> ParseFieldFilter(IFilter filters)
        {
            var op = filters.GetType().GetProperty("Operator")?.GetValue(filters, null);
            var predicate = filters.GetType().GetProperty("Predicate")?.GetValue(filters, null);
            var value = filters.GetType().GetProperty("Value")?.GetValue(filters, null);
            var not = filters.GetType().GetProperty("Not")?.GetValue(filters, null);
            var field = filters.GetType().GetProperty("FieldName")?.GetValue(filters, null);
            var ignoreCase = filters.GetType().GetProperty("IgnoreCase")?.GetValue(filters, null);

            return new FieldPredicate<T>
            {
                PropertyName = field != null
                    ? field.ToString()
                    : (ReflectionHelper.GetProperty(predicate as Expression<Func<T, object>>) as PropertyInfo)?.Name,
                Operator = (Operator)(int)(ConditionOperator)op,
                Value = value,
                Not = (bool)not,
                IgnoreCase = (bool)ignoreCase
            };
        }

        private IPredicate PreparePredicate(IFilter filters)
        {
            if (filters == null)
                return null;
            IPredicate predicateParam = null;
            if (filters is IFilterField<T>)
            {
                predicateParam = ParseFieldFilter(filters);
            }
            else if (filters is IFilterGroup)
            {
                predicateParam = new PredicateGroup
                {
                    Predicates = new List<IPredicate>(),
                    Operator = (GroupOperator)(filters as IFilterGroup).Operator
                };

                ParseGroupPredicate(filters, (PredicateGroup)predicateParam);
            }
            else if (filters is IExistsFilter<T>)
            {
                predicateParam = ParseExistsFilter(filters);
            }

            return predicateParam;
        }

        private IPredicate ParseExistsFilter(IFilter filters)
        {
            var existsField = (filters as IExistsFilter<T>);
            var predicate = this.PreparePredicate(existsField.filter);

            return Predicates.Exists<T>(predicate, existsField.Not);
        }

        #endregion

        #region Polly
        private JsonParameter ConvertDataTableToJson(DataTable table)
        {
            foreach (DataColumn column in table.Columns)
                column.ColumnName = column.ColumnName.ToLower();
            return new JsonParameter(JsonConvert.SerializeObject(table, Formatting.Indented));
        }
        private CommonDbException CreateDatabaseExeception(SqlException ex)
        {
            IEnumerable<string> errorCodes = ex.Errors.OfType<SqlError>().Select(error => error.Number.ToString()).ToList();
            errorCodes.Append(ex.Number.ToString());
            return new CommonDbException(ex.Message, errorCodes, false, ex);
        }

        private CommonDbException CreateDatabaseExeception(NpgsqlException ex)
        {
            PostgresException pex = ex as PostgresException;
            bool isTransient = ex.IsTransient || (pex?.IsTransient ?? false);
            List<string> errorCodes = new List<string>
            {
                pex?.SqlState.ToString()?? ex.ErrorCode.ToString()
            };
            return new CommonDbException(ex.Message, errorCodes, isTransient,
                ex, CommonDbException.DBEngine.PostgreSQL);
        }

        private TResult PollyExecute<TResult>(Func<TResult> action)
        {
            return _policyWrap.Execute((ctx) =>
           {
               try
               {
                   return action();
               }
               catch (NpgsqlException ex)
               {
                   throw CreateDatabaseExeception(ex);
               }
               catch (SqlException ex)
               {
                   throw CreateDatabaseExeception(ex);
               }
           }, _context);
        }

        private async Task<TResult> PollyExecuteAsync<TResult>(Func<Task<TResult>> action)
        {
            return await _asyncPolicyWrap.ExecuteAsync(async (ctx) =>
            {
                try
                {
                    return await action().ConfigureAwait(false);
                }
                catch (NpgsqlException ex)
                {
                    throw CreateDatabaseExeception(ex);
                }
                catch (SqlException ex)
                {
                    throw CreateDatabaseExeception(ex);
                }
            }, _context).ConfigureAwait(false);
        }

        private async Task<(IEnumerable<TResult>, long)> PollyExecuteAsync<TResult>(Func<Task<(IEnumerable<TResult>, long)>> action)
        {
            return await _asyncPolicyWrap.ExecuteAsync(async (ctx) =>
            {
                try
                {
                    return await action().ConfigureAwait(false);
                }
                catch (NpgsqlException ex)
                {
                    throw CreateDatabaseExeception(ex);
                }
                catch (SqlException ex)
                {
                    throw CreateDatabaseExeception(ex);
                }
            }, _context).ConfigureAwait(false);
        }
        #endregion Polly
    }
}
