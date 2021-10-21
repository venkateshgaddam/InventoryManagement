using Carrier.logging.Service;
using System.Data;
using System.Data.Common;

namespace Carrier.CCP.Common.Repository
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class LogDbConnection : DbConnection
    {
        private readonly IPlatformLogger _logger;

        /// <summary>
        /// 
        /// </summary>
        protected DbConnection Connection;
        private bool _disposed;

        /// <summary>
        /// 
        /// </summary>
        public override string ConnectionString
        {
            get => Connection.ConnectionString;
            set => Connection.ConnectionString = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Database => Connection.Database;

        /// <summary>
        /// 
        /// </summary>
        public override string DataSource => Connection.DataSource;

        /// <summary>
        /// 
        /// </summary>
        public override string ServerVersion => Connection.ServerVersion;

        /// <summary>
        /// 
        /// </summary>
        public override ConnectionState State => Connection.State;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        protected LogDbConnection(IPlatformLogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        ~LogDbConnection() => Dispose(false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Disposing"></param>
        protected override void Dispose(bool Disposing)
        {
            if (_disposed) return;
            if (Disposing)
            {
                // No managed resources to release.
            }
            // Release unmanaged resources.
            Connection?.Dispose();
            Connection = null;
            // Do not release logger.  Its lifetime is controlled by caller.
            _disposed = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DatabaseName"></param>
        public override void ChangeDatabase(string DatabaseName)
        {
            Connection.ChangeDatabase(DatabaseName);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Close()
        {
            Connection.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Open()
        {
            Connection.Open();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IsolationLevel"></param>
        /// <returns></returns>
        protected override DbTransaction BeginDbTransaction(IsolationLevel IsolationLevel)
        {
            return Connection.BeginTransaction();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override DbCommand CreateDbCommand()
        {
            if (this.Connection.State != ConnectionState.Open)
                this.Connection.Open();

            return new LogDbCommand(_logger, Connection.CreateCommand());
        }
    }
}
