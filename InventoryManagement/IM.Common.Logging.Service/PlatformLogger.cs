using Amazon.Kinesis;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.Amazon.Kinesis.Common;
using Serilog.Sinks.Amazon.Kinesis.Stream;
using System;


namespace IM.Common.Logging.Service
{
    public class PlatformLogger : IPlatformLogger
    {
        private static ILogger Logger;
        private static string AppInsightskey = string.Empty;
        private readonly IHttpContextAccessor HttpContextAccessor;
        private readonly string KinesisStreamName = string.Empty;
        private readonly bool SaveToAppInsights;
        private readonly string[] Throttle;

        // static LoggingLevelSwitch levelSwitch = null;
        public PlatformLogger(IOptions<ConfigurationSec> config, IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
            AppInsightskey = config.Value.AppInsightsKey;
            SaveToAppInsights = bool.Parse(config.Value.SaveToAppInsights);
            KinesisStreamName = config.Value.KinesisStreamName;
            Throttle = config.Value.Throttle?.Split(',');
        }

        public ILogger InstanceLogger
        {
            get
            {
                var levelSwitch = new LoggingLevelSwitch
                {
                    MinimumLevel = 0
                };
                if (SaveToAppInsights)
                {
                    var tele = new TeleConverter(HttpContextAccessor.HttpContext, Throttle);
                    Logger = new LoggerConfiguration()
                        .MinimumLevel
                        .ControlledBy(levelSwitch)
                        .WriteTo.File("", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fffzz} [{Level}]  ({CorrelationId}) {Message}{NewLine}{Exception}").CreateLogger();

                    //.ApplicationInsights(AppInsightskey, tele).CreateLogger();
                }
                else
                {
                    try
                    {
                        var client = new AmazonKinesisClient();
                        Logger = new LoggerConfiguration()
                            .MinimumLevel
                            .ControlledBy(levelSwitch)
                            .WriteTo
                            .AmazonKinesis(
                                client,
                                KinesisStreamName,
                                period: TimeSpan.FromSeconds(2)
                            ).CreateLogger();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                        //Save to DB or other stores
                    }
                }

                return Logger;
            }
        }

        private void OnLogSendError(object sender, LogSendErrorEventArgs e)
        {
            //Save to DB or other stores
            throw new NotImplementedException();
        }
    }

    public interface IPlatformLogger
    {
        ILogger InstanceLogger { get; }
    }

    #region testpurpose

    //public class CCPFileLogger
    //{
    //    public static string GetTempPath()
    //    {
    //        string path = System.Environment.GetEnvironmentVariable("TEMP");
    //        if (!path.EndsWith("\\")) path += "\\";
    //        return path;
    //    }

    //    public static void LogMessageToFile(string msg)
    //    {
    //        System.IO.StreamWriter sw = System.IO.File.AppendText(
    //            GetTempPath() + "SentinelLogFile.html");
    //        try
    //        {
    //            string logLine = System.String.Format(
    //                "{0:G}: {1}.", System.DateTime.Now, msg);
    //            sw.WriteLine(logLine);
    //        }
    //        finally
    //        {
    //            sw.Close();
    //        }
    //    }
    //}

    #endregion

    public class TelemetryException : Exception
    {
        public new Exception InnerException { get; set; }
    }
}