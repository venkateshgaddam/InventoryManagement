using DapperExtensions.Mapper;
using DapperExtensions.Sql;
using System;
using System.Collections.Generic;
using System.Data;

namespace DapperExtensions
{
    public interface IDatabase : IDisposable
    {
        bool HasActiveTransaction { get; }
        IDbConnection Connection { get; }
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        void Commit();
        void Rollback();
        void RunInTransaction(Action action);
        T RunInTransaction<T>(Func<T> func);
        T Get<T>(dynamic id, IDbTransaction transaction, int? commandTimeout = null, string schema = null) where T : class;
        T Get<T>(dynamic id, int? commandTimeout = null, string schema = null) where T : class;
        int Insert<T>(IEnumerable<T> entities, IDbTransaction transaction, int? commandTimeout = null, string schema = null) where T : class;
        int Insert<T>(IEnumerable<T> entities, int? commandTimeout = null, string schema = null) where T : class;
        dynamic Insert<T>(T entity, IDbTransaction transaction, int? commandTimeout = null, string schema = null) where T : class;
        dynamic Insert<T>(T entity, int? commandTimeout = null, string schema = null) where T : class;
        bool Update<T>(T entity, IDbTransaction transaction, int? commandTimeout = null, bool ignoreAllKeyProperties = false, string schema = null) where T : class;
        bool Update<T>(T entity, int? commandTimeout = null, bool ignoreAllKeyProperties = false, string schema = null) where T : class;
        bool Delete<T>(T entity, IDbTransaction transaction, int? commandTimeout = null, string schema = null) where T : class;
        bool Delete<T>(T entity, int? commandTimeout = null, string schema = null) where T : class;
        bool Delete<T>(object predicate, IDbTransaction transaction, int? commandTimeout = null, string schema = null) where T : class;
        bool Delete<T>(object predicate, int? commandTimeout = null, string schema = null) where T : class;
        IEnumerable<T> GetList<T>(object predicate, IList<ISort> sort, IDbTransaction transaction, int? commandTimeout = null, bool buffered = true, string schema = null) where T : class;
        IEnumerable<T> GetList<T>(object predicate = null, IList<ISort> sort = null, int? commandTimeout = null, bool buffered = true, string schema = null) where T : class;
        IEnumerable<T> GetPage<T>(object predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction, int? commandTimeout = null, bool buffered = true, string schema = null) where T : class;
        IEnumerable<T> GetPage<T>(object predicate, IList<ISort> sort, int page, int resultsPerPage, int? commandTimeout = null, bool buffered = true, string schema = null) where T : class;
        IEnumerable<T> GetSet<T>(object predicate, IList<ISort> sort, int firstResult, int maxResults, IDbTransaction transaction, int? commandTimeout, bool buffered, string schema = null) where T : class;
        IEnumerable<T> GetSet<T>(object predicate, IList<ISort> sort, int firstResult, int maxResults, int? commandTimeout, bool buffered, string schema = null) where T : class;
        int Count<T>(object predicate, IDbTransaction transaction, int? commandTimeout = null, string schema = null) where T : class;
        int Count<T>(object predicate, int? commandTimeout = null, string schema = null) where T : class;
        IMultipleResultReader GetMultiple(GetMultiplePredicate predicate, IDbTransaction transaction, int? commandTimeout = null, string schema = null);
        IMultipleResultReader GetMultiple(GetMultiplePredicate predicate, int? commandTimeout = null, string schema = null);
        void ClearCache();
        Guid GetNextGuid();
        IClassMapper GetMap<T>() where T : class;
    }

    public class Database : IDatabase
    {
        private readonly IDapperImplementor _dapper;

        private IDbTransaction _transaction;

        public Database(IDbConnection connection, ISqlGenerator sqlGenerator)
        {
            _dapper = new DapperImplementor(sqlGenerator);
            Connection = connection;

            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }
        }

        public bool HasActiveTransaction
        {
            get
            {
                return _transaction != null;
            }
        }

        public IDbConnection Connection { get; private set; }

        public void Dispose()
        {
            if (Connection.State != ConnectionState.Closed)
            {
                if (_transaction != null)
                {
                    _transaction.Rollback();
                }

                Connection.Close();
            }
        }

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            _transaction = Connection.BeginTransaction(isolationLevel);
        }

        public void Commit()
        {
            _transaction.Commit();
            _transaction = null;
        }

        public void Rollback()
        {
            _transaction.Rollback();
            _transaction = null;
        }

        public void RunInTransaction(Action action)
        {
            BeginTransaction();
            try
            {
                action();
                Commit();
            }
            catch (Exception ex)
            {
                if (HasActiveTransaction)
                {
                    Rollback();
                }

                throw ex;
            }
        }

        public T RunInTransaction<T>(Func<T> func)
        {
            BeginTransaction();
            try
            {
                T result = func();
                Commit();
                return result;
            }
            catch (Exception ex)
            {
                if (HasActiveTransaction)
                {
                    Rollback();
                }

                throw ex;
            }
        }

        public T Get<T>(dynamic id, IDbTransaction transaction, int? commandTimeout, string schema) where T : class
        {
            return (T)_dapper.Get<T>(Connection, id, transaction, commandTimeout, schema);
        }

        public T Get<T>(dynamic id, int? commandTimeout, string schema) where T : class
        {
            return (T)_dapper.Get<T>(Connection, id, _transaction, commandTimeout, schema);
        }

        public int Insert<T>(IEnumerable<T> entities, IDbTransaction transaction, int? commandTimeout, string schema) where T : class
        {
            return _dapper.Insert<T>(Connection, entities, transaction, commandTimeout, schema);
        }

        public int Insert<T>(IEnumerable<T> entities, int? commandTimeout, string schema) where T : class
        {
            return _dapper.Insert<T>(Connection, entities, _transaction, commandTimeout, schema);
        }

        public dynamic Insert<T>(T entity, IDbTransaction transaction, int? commandTimeout, string schema) where T : class
        {
            return _dapper.Insert<T>(Connection, entity, transaction, commandTimeout, schema);
        }

        public dynamic Insert<T>(T entity, int? commandTimeout, string schema) where T : class
        {
            return _dapper.Insert<T>(Connection, entity, _transaction, commandTimeout, schema);
        }

        public bool Update<T>(T entity, IDbTransaction transaction, int? commandTimeout, bool ignoreAllKeyProperties, string schema) where T : class
        {
            return _dapper.Update<T>(Connection, entity, transaction, commandTimeout, ignoreAllKeyProperties, schema);
        }

        public bool Update<T>(T entity, int? commandTimeout, bool ignoreAllKeyProperties, string schema) where T : class
        {
            return _dapper.Update<T>(Connection, entity, _transaction, commandTimeout, ignoreAllKeyProperties, schema);
        }

        public bool Delete<T>(T entity, IDbTransaction transaction, int? commandTimeout, string schema) where T : class
        {
            return _dapper.Delete(Connection, entity, transaction, commandTimeout, schema);
        }

        public bool Delete<T>(T entity, int? commandTimeout, string schema) where T : class
        {
            return _dapper.Delete(Connection, entity, _transaction, commandTimeout, schema);
        }

        public bool Delete<T>(object predicate, IDbTransaction transaction, int? commandTimeout, string schema) where T : class
        {
            return _dapper.Delete<T>(Connection, predicate, transaction, commandTimeout, schema);
        }

        public bool Delete<T>(object predicate, int? commandTimeout, string schema) where T : class
        {
            return _dapper.Delete<T>(Connection, predicate, _transaction, commandTimeout, schema);
        }

        public IEnumerable<T> GetList<T>(object predicate, IList<ISort> sort, IDbTransaction transaction, int? commandTimeout, bool buffered, string schema) where T : class
        {
            return _dapper.GetList<T>(Connection, predicate, sort, transaction, commandTimeout, buffered, schema);
        }

        public IEnumerable<T> GetList<T>(object predicate, IList<ISort> sort, int? commandTimeout, bool buffered, string schema) where T : class
        {
            return _dapper.GetList<T>(Connection, predicate, sort, _transaction, commandTimeout, buffered, schema);
        }

        public IEnumerable<T> GetPage<T>(object predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction, int? commandTimeout, bool buffered, string schema) where T : class
        {
            return _dapper.GetPage<T>(Connection, predicate, sort, page, resultsPerPage, transaction, commandTimeout, buffered, schema);
        }

        public IEnumerable<T> GetPage<T>(object predicate, IList<ISort> sort, int page, int resultsPerPage, int? commandTimeout, bool buffered, string schema) where T : class
        {
            return _dapper.GetPage<T>(Connection, predicate, sort, page, resultsPerPage, _transaction, commandTimeout, buffered, schema);
        }

        public IEnumerable<T> GetSet<T>(object predicate, IList<ISort> sort, int firstResult, int maxResults, IDbTransaction transaction, int? commandTimeout, bool buffered, string schema) where T : class
        {
            return _dapper.GetSet<T>(Connection, predicate, sort, firstResult, maxResults, transaction, commandTimeout, buffered, schema);
        }

        public IEnumerable<T> GetSet<T>(object predicate, IList<ISort> sort, int firstResult, int maxResults, int? commandTimeout, bool buffered, string schema) where T : class
        {
            return _dapper.GetSet<T>(Connection, predicate, sort, firstResult, maxResults, _transaction, commandTimeout, buffered, schema);
        }

        public int Count<T>(object predicate, IDbTransaction transaction, int? commandTimeout, string schema) where T : class
        {
            return _dapper.Count<T>(Connection, predicate, transaction, commandTimeout, schema);
        }

        public int Count<T>(object predicate, int? commandTimeout, string schema) where T : class
        {
            return _dapper.Count<T>(Connection, predicate, _transaction, commandTimeout, schema);
        }

        public IMultipleResultReader GetMultiple(GetMultiplePredicate predicate, IDbTransaction transaction, int? commandTimeout, string schema)
        {
            return _dapper.GetMultiple(Connection, predicate, transaction, commandTimeout, schema);
        }

        public IMultipleResultReader GetMultiple(GetMultiplePredicate predicate, int? commandTimeout, string schema)
        {
            return _dapper.GetMultiple(Connection, predicate, _transaction, commandTimeout, schema);
        }

        public void ClearCache()
        {
            _dapper.SqlGenerator.Configuration.ClearCache();
        }

        public Guid GetNextGuid()
        {
            return _dapper.SqlGenerator.Configuration.GetNextGuid();
        }

        public IClassMapper GetMap<T>() where T : class
        {
            return _dapper.SqlGenerator.Configuration.GetMap<T>();
        }
    }
}