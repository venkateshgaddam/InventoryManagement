namespace IM.Common.Utils.Exception
{
    public class CcpBaseException : System.Exception
    {
        public CcpBaseException() : this(CcpErrorStatus.Internal, ErrorConstants.ERRORSTATUS_INTERNAL, string.Empty)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="status">ErrorStatus enumeration</param>
        /// <param name="message">error message</param>
        /// <param name="reference">reference string code for app error. e.g. A1002</param>
        public CcpBaseException(CcpErrorStatus status, string message, string reference) : base(message)
        {
            Reference = reference;
            Status = status;
            Code = status.GetHttpCode();
        }

        /// <summary>
        /// </summary>
        /// <param name="status">ErrorStatus enumeration</param>
        /// <param name="message">error message</param>
        public CcpBaseException(CcpErrorStatus status, string message) : this(status, message, string.Empty)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="status">ErrorStatus enumeration</param>
        /// <param name="message">error message</param>
        /// <param name="reference">reference string code for app error. e.g. A1002</param>
        /// <param name="ex">Actual Exception </param>
        public CcpBaseException(CcpErrorStatus status, string message, string reference, System.Exception ex) : base(
            message, ex)
        {
            Reference = reference;
            Status = status;
            Code = status.GetHttpCode();
        }

        /// <summary>
        /// </summary>
        /// <param name="status">ErrorStatus enumeration</param>
        public CcpBaseException(CcpErrorStatus status) : this(status, status.GetDescription())
        {
        }

        public CcpBaseException(string message) : this(CcpErrorStatus.Internal, message)
        {
        }

        public CcpBaseException(string message, System.Exception innerException)
            : this(CcpErrorStatus.Internal, innerException.Message)
        {
        }

        public CcpBaseException(int errorNumber, System.Exception innerException)
            : base(errorNumber.ToString(), innerException)
        {
        }

        public int Code { get; set; }
        public CcpErrorStatus Status { get; set; }
        public string Reference { get; set; }
    }
}