using System;
using System.Collections.Generic;

namespace IM.Common.logging.Service.Models
{
    public class MetricLog
    {
        private Dictionary<string, string> _Properties;

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

        public long StartTime{ get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public TimeSpan Duration { get; set; }

        public MetricLog SetName(string message)
        {
            Name = message;
            return this;
        }

        public MetricLog SetDuration(TimeSpan duration)
        {
            Duration = duration;
            return this;
        }

        public MetricLog SetStartTime(long startTime)
        {
            StartTime = startTime;
            return this;
        }

        public MetricLog SetValue(double value)
        {
            Value = value;
            return this;
        }

        public MetricLog SetProperties(string key, string value)
        {
            Properties.Add(key, value);
            return this;
        }
    }
}