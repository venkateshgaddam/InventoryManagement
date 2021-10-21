namespace Carrier.CCP.Common.Utils.Exception
{
    public class CcpHttpException : CcpBaseException
    {
        public CcpHttpException(int httpStatusCode, string message, string reference)
            : base(httpStatusCode.CreateCCPErrorStatusFromHttpCode(), message, reference)
        {
        }

        public CcpHttpException(CcpErrorStatus status, string message, string reference)
            : base(status, message, reference)
        {
        }

        public CcpHttpException()
        {
        }

        public CcpHttpException(string message) : base(message)
        {
        }

        public CcpHttpException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}