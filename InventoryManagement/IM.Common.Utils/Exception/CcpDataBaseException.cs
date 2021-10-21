namespace IM.Common.Utils.Exception
{
    public class CcpDataBaseException : CcpBaseException
    {
        //public CcpSQLException(int httpStatusCode, string message, string reference)
        //    : base(httpStatusCode.CreateCCPErrorStatusFromHttpCode(), message, reference)
        //{
        //}

        //public CcpSQLException(CcpErrorStatus status, string message, string reference)
        //    : base(status, message, reference)
        //{
        //}
        public CcpDataBaseException(string message) : base(message)
        {
        }

        public CcpDataBaseException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        public CcpDataBaseException(int errorNumber, System.Exception innerException) : base(errorNumber, innerException)
        {
        }
    }
}