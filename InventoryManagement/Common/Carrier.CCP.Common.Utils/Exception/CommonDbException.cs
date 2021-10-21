using System.Collections.Generic;
using System.Data.Common;

namespace Carrier.CCP.Common.Utils.Exception
{
    //
    // Summary:
    //     The exception that is thrown when SQL (PostgreSQL or MS Sql or MySQL) Server returns a warning or error. This
    //     class cannot be inherited.
    public sealed class CommonDbException : DbException
    {
        public enum DBEngine
        {
            SqlServer,
            PostgreSQL
        }
        
        private readonly List<string> _errorCodes;

        public CommonDbException()
        {
            _errorCodes = new List<string>();
            this.ActiveDBEngine = DBEngine.SqlServer;
        }

        public CommonDbException(string message, IEnumerable<string> errorCodes, bool isTransient,
            System.Exception innerException, DBEngine dbEngine = DBEngine.SqlServer) : base(message, innerException)
        {
            _errorCodes = new List<string>();
            _errorCodes.AddRange(errorCodes);
            this.IsTransient = isTransient;
            this.ActiveDBEngine = dbEngine;
        }

        public CommonDbException(string message, IEnumerable<string> errorCodes,
            bool isTransient, DBEngine dbEngine = DBEngine.SqlServer) : base(message)
        {
            _errorCodes = new List<string>();
            _errorCodes.AddRange(errorCodes);
            this.IsTransient = isTransient;
            this.ActiveDBEngine = dbEngine;
        }

        //
        // Summary:
        //     Gets a HEX number that identifies the type of error.
        //
        // Value:
        //     The HEX number that identifies the type of error.
        public IEnumerable<string> ErrorCodes { get { return _errorCodes; } }

        //Only applicable for PostgreSQL
        public bool IsTransient { get; }

        //Active DB Engine
        public DBEngine ActiveDBEngine { get; }
    }
}
