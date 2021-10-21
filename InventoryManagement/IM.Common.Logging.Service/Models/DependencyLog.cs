using System;
using System.Collections.Generic;

namespace IM.Common.logging.Service.Models
{
    public class DependencyLog
    {
        private Dictionary<string, string> _Properties;
        public long StartTime { get; set; }
        public string Name { get; set; }
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