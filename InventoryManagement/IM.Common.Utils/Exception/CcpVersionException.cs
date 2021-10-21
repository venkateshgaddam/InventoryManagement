namespace IM.Common.Utils.Exception
{
    public class CcpVersionException : CcpBaseException
    {
        public CcpVersionException(VersionErrorCode versionError)
            : base(CcpErrorStatus.InvalidArgument,
                versionError.GetAttributeValue<ErrorReferenceAttribute, ErrorReferenceData>().Description,
                versionError.GetAttributeValue<ErrorReferenceAttribute, ErrorReferenceData>().Code)
        {
        }

        public CcpVersionException() : this(VersionErrorCode.UnspecifiedError)
        {
        }

        public CcpVersionException(string message) : base(message)
        {
        }

        public CcpVersionException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}