using System;
using System.Collections.Generic;

namespace IM.Common.logging.Service.Models
{
    public class RequestLog
    {
        private Dictionary<string, string> _Properties;
        public long StartTime { get; set; }
        public string Name { get; set; }
        public string HttpMethod { get; set; }
        public TimeSpan Duration { get; set; }
        public string ResponseCode { get; set; }
        public Uri Url { get; set; }

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