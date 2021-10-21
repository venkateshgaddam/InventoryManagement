using System;
using System.Collections.Generic;

namespace IM.Common.logging.Service.Models
{
    public class ExceptionLog
    {
        private Dictionary<string, string> _Properties;
        public Exception Exception { get; set; }
        public string MInnerException { get; set; }
        public string MstrExternalMessageL1 { get; set; }
        public string MstrExternalMessageL2 { get; set; }
        public string MErrorCodeL1 { get; set; }
        public string MErrorCodeL2 { get; set; }
        public string MappingHttpErrorCode { get; set; }
        public string SystemIdentifier { get; set; }
        public string StackTrace { get; set; }
        public string Message { get; set; }
        public string TargetSite { get; set; }
        public string OperationName { get; set; }
        public string UserAgent { get; set; }
        public string IsmobileDevice { get; set; }
        public string MobileDeviceManufacturer { get; set; }
        public string RoleInstance { get; set; }
        public string IPAddress { get; set; }
        public long StartTime { get; set; }
        public TimeSpan Duration { get; set; }

        public Dictionary<string, string> Properties
        {
            get
            {
                if (_Properties != null)
                {
                    return _Properties;
                }

                _Properties = new Dictionary<string, string>();
                return _Properties;
            }
            set { _Properties = value; }
        }
    }
}