namespace Carrier.CCP.Common.Utils.Exception
{
    public class CcpValidationException : CcpBaseException
    {
        public CcpValidationException(int httpStatusCode, string message, string reference)
            : base(httpStatusCode.CreateCCPErrorStatusFromHttpCode(), message, reference)
        {
        }

        public CcpValidationException(CcpErrorStatus status, string message, string reference)
            : base(status, message, reference)
        {
        }

        public CcpValidationException()
        {
        }

        public CcpValidationException(string message) : base(message)
        {
        }

        public CcpValidationException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}