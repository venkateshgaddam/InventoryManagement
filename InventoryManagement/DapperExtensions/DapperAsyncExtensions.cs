using DapperExtensions.Mapper;
using DapperExtensions.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;

namespace DapperExtensions
{
    public static class DapperAsyncExtensions
    {
        private readonly static object _lock = new object();

        private static Func<IDapperExtensionsConfiguration, IDapperAsyncImplementor> _instanceFactory;
        private static IDapperAsyncImplementor _instance;
        private static IDapperExtensionsConfiguration _configuration;

        /// <summary>
        /// Gets or sets the default class mapper to use when generating class maps. If not specified, AutoClassMapper{T} is used.
        /// DapperExtensions.Configure(Type, IList{Assembly}, ISqlDialect) can be used instead to set all values at once
        /// </summary>
        public static Type DefaultMapper
        {
            get
            {
                return _configuration.DefaultMapper;
            }

            set
            {
                Configure(value, _configuration.MappingAssemblies, _configuration.Dialect);
            }
        }

        /// <summary>
        /// Gets or sets the type of sql to be generated.
        /// DapperExtensions.Configure(Type, IList{Assembly}, ISqlDialect) can be used instead to set all values at once
        /// </summary>
        public static ISqlDialect SqlDialect
        {
            get
            {
                return _configuration.Dialect;
            }

            set
            {
                Configure(_configuration.DefaultMapper, _configuration.MappingAssemblies, value);
            }
        }

        /// <summary>
        /// Get or sets the Dapper Extensions Implementation Factory.
        /// </summary>
        public static Func<IDapperExtensionsConfiguration, IDapperAsyncImplementor> InstanceFactory
        {
            get
            {
                if (_instanceFactory == null)
                {
                    _instanceFactory = config => new DapperAsyncImplementor(new SqlGeneratorImpl(config));
                }

                return _instanceFactory;
            }
            set
            {
                _instanceFactory = value;
                Configure(_configuration.DefaultMapper, _configuration.MappingAssemblies, _configuration.Dialect);
            }
        }

        /// <summary>
        /// Gets the Dapper Extensions Implementation
        /// </summary>
        private static IDapperAsyncImplementor Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = InstanceFactory(_configuration);
                        }
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Initializes the <see cref="DapperAsyncExtensions"/> class.
        /// </summary>
        static DapperAsyncExtensions()
        {
            Configure(typeof(AutoClassMapper<>), new List<Assembly>(), new SqlServerDialect());
        }

        /// <summary>
        /// Add other assemblies that Dapper Extensions will search if a mapping is not found in the same assembly of the POCO.
        /// </summary>
        /// <param name="assemblies"></param>
        public static void SetMappingAssemblies(IList<Assembly> assemblies)
        {
            Configure(_configuration.DefaultMapper, assemblies, _configuration.Dialect);
        }

        /// <summary>
        /// Configure DapperExtensions extension methods.
        /// </summary>
        public static void Configure(IDapperExtensionsConfiguration configuration)
        {
            _instance = null;
            _configuration = configuration;
        }

        /// <summary>
        /// Configure DapperExtensions extension methods.
        /// </summary>
        /// <param name="defaultMapper"></param>
        /// <param name="mappingAssemblies"></param>
        /// <param name="sqlDialect"></param>
        public static void Configure(Type defaultMapper, IList<Assembly> mappingAssemblies, ISqlDialect sqlDialect)
        {
            Configure(new DapperExtensionsConfiguration(defaultMapper, mappingAssemblies, sqlDialect));
        }

        /// <summary>
        /// Executes a query using the specified predicate, returning an integer that represents the number of rows that match the query.
        /// </summary>
        public static async Task<int> CountAsync<T>(this IDbConnection connection, object predicate = null, IDbTransaction transaction = null, int? commandTimeout = null, string schema = null) where T : class
        {
            return await Instance.CountAsync<T>(connection, predicate, transaction, commandTimeout, schema);
        }

        /// <summary>
        /// Executes a query for the specified id, returning the data typed as per T.
        /// </summary>
        public static async Task<T> GetAsync<T>(this IDbConnection connection, dynamic id, IDbTransaction transaction = null, int? commandTimeout = null, string schema = null, IList<string> selectPropertyName = null) where T : class
        {
            return await Instance.GetAsync<T>(connection, id, transaction, commandTimeout, schema, selectPropertyName);
        }

        /// <summary>
        /// Executes a select query using the specified predicate, returning an IEnumerable data typed as per T.
        /// </summary>
        public static async Task<IEnumerable<T>> GetListAsync<T>(this IDbConnection connection, object predicate = null, IList<ISort> sort = null, IDbTransaction transaction = null, int? commandTimeout = null, string schema = null, IList<string> selectPropertyName = null, bool distinct = false) where T : class
        {
            return await Instance.GetListAsync<T>(connection, predicate, sort, transaction, commandTimeout, schema, selectPropertyName, distinct);
        }
        /// <summary>
        /// Executes an insert query for the specified entity.
        /// </summary>
        public static Task<int> InsertAsync<T>(this IDbConnection connection, IEnumerable<T> entities, IDbTransaction transaction = null, int? commandTimeout = default, string schema = null) where T : class
        {
            return Instance.InsertAsync(connection, entities, transaction, commandTimeout, schema);
        }
        /// <summary>
        /// Executes an insert query for the specified entity, returning the primary key.  
        /// If the entity has a single key, just the value is returned.  
        /// If the entity has a composite key, an IDictionary&lt;string, object&gt; is returned with the key values.
        /// The key value for the entity will also be updated if the KeyType is a Guid or Identity.
        /// </summary>
        public static Task<dynamic> InsertAsync<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? commandTimeout = default, string schema = null) where T : class
        {
            return Instance.InsertAsync(connection, entity, transaction, commandTimeout, schema);
        }
        /// <summary>
        /// Executes an update query for the specified entity.
        /// </summary>
        public static Task<T> UpdateAsync<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? commandTimeout = null, bool ignoreAllKeyProperties = false, IPredicate predicate = null, IPredicate existsPredicate = null, string schema = null) where T : class
        {
            return Instance.UpdateAsync(connection, entity, transaction, commandTimeout, ignoreAllKeyProperties, predicate, existsPredicate, schema);
        }
        /// <summary>
        /// Executes a delete query for the specified entity.
        /// </summary>
        public static Task<int> DeleteByIdAsync<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? commandTimeout = null, string schema = null) where T : class
        {
            return Instance.DeleteByIdAsync(connection, entity, transaction, commandTimeout, schema);
        }
        /// <summary>
        /// Executes a delete query using the specified predicate.
        /// </summary>
        public static Task<int> DeleteAsync<T>(this IDbConnection connection, object predicate, IDbTransaction transaction = null, int? commandTimeout = null, string schema = null) where T : class
        {
            return Instance.DeleteAsync<T>(connection, predicate, transaction, commandTimeout, schema);
        }

        /// <summary>
        /// Executes a select query using the specified predicate, returning an IEnumerable data typed as per T.
        /// Data returned is dependent upon the specified page and resultsPerPage.
        /// </summary>
        public static async Task<(IEnumerable<T>, long)> GetPageAsync<T>(this IDbConnection connection, object predicate = null, IList<ISort> sort = null, int page = 1, int resultsPerPage = 10, IDbTransaction transaction = null, int? commandTimeout = null, string schema = null, IList<string> selectPropertyName = null, bool distinct = false) where T : class
        {
            return await Instance.GetPageAsync<T>(connection, predicate, sort, page, resultsPerPage, transaction, commandTimeout, schema, selectPropertyName, distinct);
        }


        /// <summary>
        /// Executes a select query using the specified predicate, returning an IEnumerable data typed as per T.
        /// Data returned is dependent upon the specified firstResult and maxResults.
        /// </summary>
        public static async Task<IEnumerable<T>> GetSetAsync<T>(this IDbConnection connection, object predicate = null, IList<ISort> sort = null, int firstResult = 1, int maxResults = 10, IDbTransaction transaction = null, int? commandTimeout = null, string schema = null) where T : class
        {
            return await Instance.GetSetAsync<T>(connection, predicate, sort, firstResult, maxResults, transaction, commandTimeout, schema);
        }

        //CCP Code
        #region CCP
        /// <summary>
        /// Get the SQL generator
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static ISqlGenerator SqlQueryGenerator(this IDbConnection connection)
        {
            return Instance.SqlQueryGenerator();
        }

        /// <summary>
        /// Executes a query for the specified filter with id, returning the data typed as per T.
        /// </summary>
        public static async Task<T> GetRecordAsync<T>(this IDbConnection connection, object predicate, IDbTransaction transaction = null, int? commandTimeout = null, string schema = null, IList<string> selectPropertyName = null) where T : class
        {
            return await Instance.GetRecordAsync<T>(connection, predicate, transaction, commandTimeout, schema, selectPropertyName);
        }

        /// <summary>
        /// Executes an update query for the specified entity.
        /// </summary>
        public static Task<IEnumerable<T>> UpdateMultipleAsync<T>(this IDbConnection connection, T entity, IPredicate predicate, IEnumerable<string> columnNames, IPredicate existsPredicate = null, IDbTransaction transaction = null, int? commandTimeout = null, bool ignoreAllKeyProperties = false, string schema = null) where T : class
        {
            return Instance.UpdateMultipleAsync(connection, entity, predicate, columnNames, existsPredicate, transaction, commandTimeout, ignoreAllKeyProperties, schema);
        }

        /// <summary>
        /// Executes a insert query using the specified entity and return inserted records
        /// </summary>
        public static async Task<T> InsertEntityAsync<T>(this IDbConnection connection, T entity, object existsPredicate = null, IDbTransaction transaction = null, int? commandTimeout = null, string schema = null) where T : class
        {
            return await Instance.InsertEntityAsync<T>(connection, entity, existsPredicate, transaction, commandTimeout, schema);
        }

        /// <summary>
        /// Executes a delete query using the specified predicate and return deleted records
        /// </summary>
        public static async Task<IEnumerable<T>> DeleteSelectedAsync<T>(this IDbConnection connection, object predicate, IDbTransaction transaction = null, int? commandTimeout = null, string schema = null) where T : class
        {
            return await Instance.DeleteSelectedAsync<T>(connection, predicate, transaction, commandTimeout, schema);
        }
        #endregion CCP
    }
}
