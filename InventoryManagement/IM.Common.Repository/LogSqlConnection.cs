using IM.Common.Logging.Service;
using System.Data;
using System.Data.SqlClient;

namespace Carrier.CCP.Common.Repository
{
    /// <summary>
    /// 
    /// </summary>
    public class LogSqlConnection : LogDbConnection
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="connection"></param>
        public LogSqlConnection(IPlatformLogger logger, string connection) : base(logger)
        {
            this.Connection = new SqlConnection(connection);
            if (this.Connection.State != ConnectionState.Open)
                this.Connection.Open();
        }
    }
}
