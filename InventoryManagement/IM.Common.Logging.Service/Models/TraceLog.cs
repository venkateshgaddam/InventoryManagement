using System.Collections.Generic;

namespace IM.Common.logging.Service.Models
{
    public class TraceLog
    {
        private Dictionary<string, string> _Properties;

        public TraceLog()
        {
            _Properties = new Dictionary<string, string>();
        }

        public string Message { get; set; }
        public string User { get; set; }

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

        public TraceLog SetMessage(string message)
        {
            Message = message;
            return this;
        }

        public TraceLog SetProperties(string key, string value)
        {
            Properties.Add(key, value);
            return this;
        }

        public TraceLog SetUser(string user)
        {
            User = user;
            return this;
        }
    }
}