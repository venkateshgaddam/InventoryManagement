namespace IM.Common.Utils.Exception
{
    public enum SecurityErrorCode
    {
        [ErrorReferenceAttribute("AD000", ErrorConstants.SECURITY_UNSPECIFIED)]
        UnspecifiedError,

        [ErrorReferenceAttribute("AD001", ErrorConstants.SECURITY_FAILUREFETCHINGACTIVEDIRECTORYTOKEN)]
        FailureFetchingActiveDirectoryToken,

        [ErrorReferenceAttribute("AD002", ErrorConstants.SECURITY_NULLREFERENCEEXCEPTION)]
        NullReferenceException,

        [ErrorReferenceAttribute("AD003", ErrorConstants.SECURITY_ADALSERVICEEXCEPTION)]
        AdalServiceException,

        [ErrorReferenceAttribute("AD004", ErrorConstants.SECURITY_ADALEXCEPTION)]
        AdalException,

        [ErrorReferenceAttribute("AD005", ErrorConstants.SECURITY_INVALIDTOKEN)]
        InvalidToken,

        [ErrorReferenceAttribute("AD006", ErrorConstants.SECURITY_USERNOTFOUND)]
        UserNotFound,

        [ErrorReferenceAttribute("AD007", ErrorConstants.SECURITY_HTTPERROR)]
        HttpError,

        [ErrorReferenceAttribute("AD008", ErrorConstants.SECURITY_USERFETCHERROR)]
        UserFetchError,

        [ErrorReferenceAttribute("AD009", ErrorConstants.SECURITY_USEREXISTSERROR)]
        UserExistsError,

        [ErrorReferenceAttribute("AD0010", ErrorConstants.SECURITY_USERDELETEERROR)]
        UserDeleteError,

        [ErrorReferenceAttribute("AD0011", ErrorConstants.SECURITY_USERCREATEERROR)]
        UserCreateError
    }

    public static class SecurityErrorCodeExtensions
    {
        public static string GetErrorReferenceCode(this SecurityErrorCode status)
        {
            return status.GetAttributeValue<ErrorReferenceAttribute, ErrorReferenceData>().Code;
        }

        public static string GetErrorReferenceDescription(this SecurityErrorCode status)
        {
            return status.GetAttributeValue<ErrorReferenceAttribute, ErrorReferenceData>().Description;
        }
    }
}