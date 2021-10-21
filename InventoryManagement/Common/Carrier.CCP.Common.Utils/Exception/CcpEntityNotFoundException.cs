namespace Carrier.CCP.Common.Utils.Exception
{
    public class CcpEntityNotFoundException : CcpBaseException
    {
        public CcpEntityNotFoundException(string message, string reference)
            : base(CcpErrorStatus.NotFound, message, reference)
        {
        }

        public CcpEntityNotFoundException()
        {
        }

        public CcpEntityNotFoundException(string message) : base(message)
        {
        }

        public CcpEntityNotFoundException(string message, System.Exception innerException) : base(message,
            innerException)
        {
        }
    }
}