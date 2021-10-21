namespace IM.Common.Logging.Service
{
    public class ConfigurationSec
    {
        public string SaveToAppInsights { get; set; }
        public string SaveToKinesis { get; set; }
        public string AppInsightsKey { get; set; }
        public string KinesisKey { get; set; }
        public string KinesisPassword { get; set; }
        public string KinesisStreamName { get; set; }
        public string Throttle { get; set; }
        public bool EnableDependencyLogging { get; set; }
        public bool EnableDBQueryLogging { get; set; }
    }
}