namespace Carrier.CCP.Common.Utils.Exception
{
    public enum VersionErrorCode
    {
        [ErrorReferenceAttribute("V000", ErrorConstants.VERSION_UNSPECIFIED)]
        UnspecifiedError,

        [ErrorReferenceAttribute("V001", ErrorConstants.VERSION_INVALIDVERSIONNUMBER)]
        InvalidVersionNumber
    }
}