namespace IM.Common.Utils.Exception
{
    public class CcpSecurityException : CcpBaseException
    {
        public CcpSecurityException(SecurityErrorCode securityError)
            : base(CcpErrorStatus.Internal,
                securityError.GetAttributeValue<ErrorReferenceAttribute, ErrorReferenceData>().Description,
                securityError.GetAttributeValue<ErrorReferenceAttribute, ErrorReferenceData>().Code)
        {
        }

        public CcpSecurityException(SecurityErrorCode securityError, string message)
            : base(CcpErrorStatus.Internal,
                message,
                securityError.GetAttributeValue<ErrorReferenceAttribute, ErrorReferenceData>().Code)
        {
        }

        public CcpSecurityException(SecurityErrorCode securityError, CcpErrorStatus status, string message)
            : base(status,
                message,
                securityError.GetAttributeValue<ErrorReferenceAttribute, ErrorReferenceData>().Code)
        {
        }

        public CcpSecurityException(CcpErrorStatus permissionDenied) : this(SecurityErrorCode.UnspecifiedError)
        {
        }

        public CcpSecurityException(string message) : this(SecurityErrorCode.UnspecifiedError, message)
        {
        }

        public CcpSecurityException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}