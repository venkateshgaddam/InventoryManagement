using System.Collections.Generic;
using System.Net;

namespace Carrier.CCP.Common.Utils.Exception
{
    public static class CcpErrorStatusExtension
    {
        private static readonly IDictionary<HttpStatusCode, CcpErrorStatus> _httpErrorCode = new Dictionary<HttpStatusCode, CcpErrorStatus>()
        {
            { HttpStatusCode.OK, CcpErrorStatus.OK },
            { HttpStatusCode.Accepted , CcpErrorStatus.OK },
            { HttpStatusCode.Ambiguous , CcpErrorStatus.FailedPrecondition },
            { HttpStatusCode.BadGateway , CcpErrorStatus.Internal },
            { HttpStatusCode.BadRequest , CcpErrorStatus.FailedPrecondition },
            { HttpStatusCode.Conflict , CcpErrorStatus.Aborted },
            { HttpStatusCode.Continue , CcpErrorStatus.OK },
            { HttpStatusCode.Created , CcpErrorStatus.OK },
            { HttpStatusCode.ExpectationFailed , CcpErrorStatus.FailedPrecondition },
            { HttpStatusCode.Forbidden , CcpErrorStatus.PermissionDenied },
            { HttpStatusCode.Found , CcpErrorStatus.OK },
            { HttpStatusCode.PartialContent , CcpErrorStatus.OK },
            { HttpStatusCode.PaymentRequired , CcpErrorStatus.InvalidArgument },
            { HttpStatusCode.PreconditionFailed , CcpErrorStatus.FailedPrecondition },
            { HttpStatusCode.ProxyAuthenticationRequired , CcpErrorStatus.Unavailable },
            { HttpStatusCode.RedirectKeepVerb , CcpErrorStatus.Unavailable },
            { HttpStatusCode.RedirectMethod , CcpErrorStatus.Unavailable },
            { HttpStatusCode.RequestedRangeNotSatisfiable , CcpErrorStatus.OutOfRange },
            { HttpStatusCode.RequestEntityTooLarge , CcpErrorStatus.ResourceExhausted },
            { HttpStatusCode.RequestTimeout , CcpErrorStatus.DeadlineExceeded },
            { HttpStatusCode.RequestUriTooLong , CcpErrorStatus.ResourceExhausted },
            { HttpStatusCode.ResetContent , CcpErrorStatus.OK },
            { HttpStatusCode.ServiceUnavailable , CcpErrorStatus.Unavailable },
            { HttpStatusCode.SwitchingProtocols , CcpErrorStatus.OK },
            { HttpStatusCode.Unauthorized , CcpErrorStatus.PermissionDenied },
            { HttpStatusCode.UnsupportedMediaType , CcpErrorStatus.FailedPrecondition },
            { HttpStatusCode.UpgradeRequired , CcpErrorStatus.OK },
            { HttpStatusCode.UseProxy , CcpErrorStatus.Unavailable },
            { HttpStatusCode.NoContent , CcpErrorStatus.NoContent },
        };

        public static CcpErrorStatus CreateCCPErrorStatusFromHttpCode(this int httpCode)
        {
            return _httpErrorCode.TryGetValue((HttpStatusCode)httpCode, out CcpErrorStatus errorStatus) ? errorStatus : CcpErrorStatus.Unknown;
        }
    }
}