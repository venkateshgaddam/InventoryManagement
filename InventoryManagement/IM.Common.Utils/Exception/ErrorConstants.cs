namespace IM.Common.Utils.Exception
{
    public class ErrorConstants
    {
        #region TokenError

        public static readonly string INVALID_TOKEN = "Invalid token";

        #endregion

        #region ErrorStatus

        internal const string ERRORSTATUS_OK = "Operation successful.";
        internal const string ERRORSTATUS_CREATED = "The request has succeeded and a new resource has been created";
        internal const string ERRORSTATUS_ACCEPTED = "The request has been accepted but not yet acted";
        internal const string ERRORSTATUS_PARTIALCONTENT = "Partial Content";
        internal const string ERRORSTATUS_CANCELLED = "The operation was cancelled, typically by the caller.";
        internal const string ERRORSTATUS_UNKNOWN = "Errors raised by APIs that do not return enough error information.";
        internal const string ERRORSTATUS_INVALIDARGUMENT = "The client specified an invalid argument.";
        internal const string ERRORSTATUS_DEADLINEEXCEEDED = "Session or Process Timeout.";

        internal const string ERRORSTATUS_NOTFOUND = "Request attempted to fetch, update or delete an entity that doesn't exist.";

        internal const string ERRORSTATUS_ALREADYEXISTS = "Request attempted to insert an entity that already exists.";
        internal const string ERRORSTATUS_PERMISSIONDENIED = "Caller not allowed to make the request.";
        internal const string ERRORSTATUS_RESOURCEEXHAUSTED = "Usage (Storage, Memory, CPU) quota exhausted.";
        internal const string ERRORSTATUS_FAILEDPRECONDITION = "Precondition for the request not met.";

        internal const string ERRORSTATUS_ABORTED = "Requested conflicted with another request or aborted for some internal reason.";

        internal const string ERRORSTATUS_OUTOFRANGE = "The operation was attempted past the valid range.";
        internal const string ERRORSTATUS_UNIMPLEMENTED = "This operation is not implemented.";
        internal const string ERRORSTATUS_INTERNAL = "Internal Server Error.";
        internal const string ERRORSTATUS_UNAVAILABLE = "Requested resource is not available at the moment.";
        internal const string ERRORSTATUS_DATALOSS = "Unrecoverable data loss or corruption.";
        internal const string ERRORSTATUS_UNAUTHENTICATED = "Request did not have valid authentication credentials.";
        internal const string ERRORSTATUS_INVALIDINPUTS = "Request does not have valid input arguments.";
        internal const string ERRORSTATUS_NOCONTENT = "The request has successfully processed.";
        internal const string ERRORSTATUS_INVALIDCONTENTTYPE = "The request has Unsupported Media Type.";
        internal const string ERRORSTATUS_UNPROCESSABLEENTITY = "The file size exceeded the limit.";
        internal const string ERRORSTATUS_DOESNOTEXIST = "Request attempted to update an entity that does not exist.";
        internal const string ERRORSTATUS_USERINTERNAL = "Request attempted to approve user who is internal.";
        internal const string ERRORSTATUS_APPROVEDUSER = "Request attempted on user who is already approved.";
        internal const string ERRORSTATUS_NOTAPPROVEDUSER = "Request attempted on user who is not approved.";
        internal const string ERRORSTATUS_ACTIVATEDUSER = "Request attempted on user who is already activated.";
        internal const string ERRORSTATUS_INACTIVEUSER = "Request attempted on user who is already inactive.";
        internal const string ERRORSTATUS_DELETEDUSER = "Request attempted on user who is already deleted.";

        #endregion

        #region VersionError

        internal const string VERSION_UNSPECIFIED = "Unspecified error.";

        internal const string VERSION_INVALIDVERSIONNUMBER = "Version cannot have negative Major, Minor, Release or Build number.";

        #endregion

        #region SecurityError

        internal const string SECURITY_UNSPECIFIED = "Unspecified error.";

        internal const string SECURITY_FAILUREFETCHINGACTIVEDIRECTORYTOKEN = "Couldn't fetch ActiveDirectoryToken using Graph API.";

        internal const string SECURITY_NULLREFERENCEEXCEPTION = "NullReferenceException while getting Access Token.";
        internal const string SECURITY_ADALSERVICEEXCEPTION = "ADALServiceException occured.";
        internal const string SECURITY_ADALEXCEPTION = "ADALException occured.";
        internal const string SECURITY_INVALIDTOKEN = "Incorrect credentials provided for token authorization.";
        internal const string SECURITY_USERNOTFOUND = "User not found.";
        internal const string SECURITY_HTTPERROR = "Http error occured.";
        internal const string SECURITY_USERFETCHERROR = "Error fetching user information.";
        internal const string SECURITY_USEREXISTSERROR = "User already exists";
        internal const string SECURITY_USERDELETEERROR = "Error Occour while delete the User";
        internal const string SECURITY_USERCREATEERROR = "Error Occour while create the User";

        #endregion
    }
}