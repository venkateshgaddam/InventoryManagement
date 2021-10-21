namespace IM.Common.Utils.Exception
{
    public class CcpHttpExceptionEx : CcpHttpException
    {
        public CcpHttpExceptionEx(int httpStatusCode, string message, string reference)
            : base(httpStatusCode.CreateCCPErrorStatusFromHttpCode(), message, reference)
        {
        }

        public CcpHttpExceptionEx(CcpErrorStatus status, string message, string reference)
            : base(status, message, reference)
        {
        }

        public CcpHttpExceptionEx()
        {
        }

        public CcpHttpExceptionEx(string message) : base(message)
        {
        }

        public CcpHttpExceptionEx(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}