using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using Serilog.Events;
using Serilog.Sinks.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IM.Common.Logging.Service
{
    /// <summary>
    ///     TeleConveter that logs into Azure App insights or AWS
    /// </summary>
    public class TeleConverter : ITelemetryConverter
    {
        private const string CORRELATIONID = "X-Correlation-ID";
        //private const string SESSIONID = "sessionId";
        private const string NAME = "Name";
        private const string RESPONSECODE = "ResponseCode";
        private const string DURATION = "Duration";
        private const string STARTTIME = "StartTime";
        private const string ENDTIME = "EndTime";
        private const string URL = "Url";
        private const string HTTPMETHOD = "HttpMethod";
        private const string VALUE = "Value";
        private const string REQUESTTELEMETRY = "RequestTelemetry";
        private const string METRICTELEMETRY = "MetricTelemetry";
        private const string EXCEPTIONTELEMETRY = "ExceptionTelemetry";
        private const string DEPENDENCYTELEMETRY = "DependencyTelemetry";
        private const string CCPLOG = "CCPLog";
        private const string CCPEVENTLOG = "CCPEventLog";
        private readonly HttpContext HttpContext;
        private readonly string[] Throttle;

        public TeleConverter(HttpContext httpContext, string[] throttle)
        {
            HttpContext = httpContext;
            Throttle = throttle;
        }

        public IEnumerable<ITelemetry> Convert(LogEvent logEvent, IFormatProvider formatProvider)
        {
            var roleInstance = GetCloudInstanceName();
            var Properties = logEvent.Properties;

            try
            {
                var UserAgent = string.Empty;
                var IsmobileDevice = string.Empty;
                var MobileDeviceManufacturer = string.Empty;
                var ipaddress = string.Empty;
                var xCorrelationId = string.Empty;
                if (HttpContext?.Request != null && HttpContext.Request.Headers != null)
                {
                    xCorrelationId = HttpContext.Request.Headers[CORRELATIONID];
                    var remoteIpAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                }

                return logEvent.MessageTemplate.Text switch
                {
                    "{@RequestTelemetry}" => ConvertRequestTelemetry(roleInstance, Properties, xCorrelationId),
                    "{@TraceTelemetry}" => ConvertTraceTelemetry(roleInstance, Properties, xCorrelationId),
                    "{@MetricTelemetry}" => ConvertMetricTelemetry(roleInstance, Properties, xCorrelationId),
                    "{@DependencyTelemetry}" => ConvertDependencyTelemetry(roleInstance, Properties, xCorrelationId),
                    "{@ExceptionTelemetry}" => ConvertExceptionTelemetry(roleInstance, Properties, xCorrelationId),
                    "{@CCPLog}" => ConvertCCPLog(roleInstance, Properties, xCorrelationId),
                    "{@CCPEventLog}" => ConvertCCPEventLog(roleInstance, Properties, xCorrelationId),
                    _ => null,
                };
            }
            catch (Exception ex)
            {
                //Log to database
                // Eat the Exception and Log one  Exception at the time of telemetry Transformations
                var list = new List<ITelemetry>();
                var exception = new ExceptionTelemetry(ex)
                {
                    Timestamp = DateTime.UtcNow,
                    Message = ex.Message
                };
                exception.Properties.Add("StackTrace", ex.StackTrace);
                exception.Properties.Add("Type", "Error");
                exception.Properties.Add("Operation", "Telemetry Transformation");
                list.Add(exception);
                return list;
            }
        }

        private IEnumerable<ITelemetry> ConvertCCPEventLog(string roleInstance, IReadOnlyDictionary<string, LogEventPropertyValue> Properties, string xCorrelationId)
        {
            // SentinelLog
            if (TryGetStructureValue(Properties, CCPEVENTLOG, out StructureValue CCPEventLogStucValue) &&
                Throttle.Contains("Event"))
            {
                var list = new List<ITelemetry>();
                var EventTrace = new EventTelemetry
                {
                    Name = GetValueFromStructureValue(CCPEventLogStucValue, "Operation")
                };
                EventTrace.Context.Device.Type =
                    GetValueFromStructureValue(CCPEventLogStucValue, "ClientId");
                EventTrace.Timestamp = DateTime.UtcNow;
                EventTrace.Properties.Add("X-Correlation-ID", xCorrelationId);
                EventTrace.Properties.Add("roleInstance", roleInstance);
                foreach (var prop in CCPEventLogStucValue.Properties)
                    if (prop.Name.Contains("Properties"))
                        EventTrace.Properties.Add(prop.Name, prop.Value.ToString());
                list.Add(EventTrace);
                return list;
            }

            return null;
        }

        private IEnumerable<ITelemetry> ConvertCCPLog(string roleInstance, IReadOnlyDictionary<string, LogEventPropertyValue> Properties, string xCorrelationId)
        {
            // SentinelLog
            if (TryGetStructureValue(Properties, CCPLOG, out StructureValue CCPLogStucValue) && Throttle.Contains("Trace"))
            {
                var list = new List<ITelemetry>();
                var trace = new TraceTelemetry
                {
                    Timestamp = DateTime.UtcNow,
                    Message = GetValueFromStructureValue(CCPLogStucValue, "Message")
                };
                trace.Properties.Add("X-Correlation-ID", xCorrelationId);
                trace.Properties.Add("roleInstance", roleInstance);
                foreach (var prop in CCPLogStucValue.Properties)
                    if (prop.Name.Contains("Properties"))
                        trace.Properties.Add(prop.Name, prop.Value.ToString());
                list.Add(trace);
                return list;
            }

            return null;
        }

        private IEnumerable<ITelemetry> ConvertExceptionTelemetry(string roleInstance, IReadOnlyDictionary<string, LogEventPropertyValue> Properties, string xCorrelationId)
        {
            //Get StructureValue
            if (TryGetStructureValue(Properties, EXCEPTIONTELEMETRY, out StructureValue exceptionStucValue) &&
                Throttle.Contains("Exception"))
            {
                var list = new List<ITelemetry>();
                var exception = new ExceptionTelemetry
                {
                    Timestamp = DateTime.UtcNow,
                    Message = GetValueFromStructureValue(exceptionStucValue, "Message")
                };

                TimeSpan duration = TimeSpan.TryParse(GetValueFromStructureValue(exceptionStucValue, DURATION), out duration) ? duration : TimeSpan.MinValue;
                DateTimeOffset startTime = DateTimeOffset.FromUnixTimeMilliseconds(GetDateTimeOffsetFromStructureValue(exceptionStucValue, STARTTIME));
                DateTimeOffset endTime = startTime.AddMilliseconds(duration.TotalMilliseconds);
                exception.Timestamp = startTime;

                exception.Properties.Add(STARTTIME, startTime.ToString("MM/dd/yyyy hh:mm:ss.fff"));
                exception.Properties.Add(ENDTIME, endTime.ToString("MM/dd/yyyy hh:mm:ss.fff"));
                //Create Exception Telemetry
                //var formater = new JsonValueFormatter();

                exception.Properties.Add("X-Correlation-ID", xCorrelationId);
                exception.Properties.Add("roleInstance", roleInstance);
                foreach (var prop in exceptionStucValue.Properties)
                    if (!exception.Properties.ContainsKey(prop.Name))
                        exception.Properties.Add(prop.Name, prop.Value.ToString());
                list.Add(exception);
                return list;
            }
            return null;
        }

        private IEnumerable<ITelemetry> ConvertDependencyTelemetry(string roleInstance, IReadOnlyDictionary<string, LogEventPropertyValue> Properties, string xCorrelationId)
        {
            //Get StructureValue
            if (TryGetStructureValue(Properties, DEPENDENCYTELEMETRY, out StructureValue dependencyvalueStucValue) &&
                Throttle.Contains("Dependency"))
            {
                var list = new List<ITelemetry>();
                //Create Metric Telemetry
                var metric = new DependencyTelemetry
                {
                    Name = GetValueFromStructureValue(dependencyvalueStucValue, NAME)
                };
                TimeSpan duration = TimeSpan.TryParse(GetValueFromStructureValue(dependencyvalueStucValue, DURATION), out duration) ? duration : TimeSpan.MinValue;
                DateTimeOffset startTime = DateTimeOffset.FromUnixTimeMilliseconds(GetDateTimeOffsetFromStructureValue(dependencyvalueStucValue, STARTTIME));
                DateTimeOffset endTime = startTime.AddMilliseconds(duration.TotalMilliseconds);
                metric.Timestamp = startTime;
                metric.Duration = duration;

                metric.Properties.Add(STARTTIME, startTime.ToString("MM/dd/yyyy hh:mm:ss.fff"));
                metric.Properties.Add(ENDTIME, endTime.ToString("MM/dd/yyyy hh:mm:ss.fff"));
                metric.Properties.Add("X-Correlation-ID", xCorrelationId);
                metric.Properties.Add("roleInstance", roleInstance);
                //Get properties and return
                //IDictionary<string, string> props = AddPropertiesToReqTel(dependencyvalueStucValue);
                foreach (var prop in dependencyvalueStucValue.Properties)
                    if (prop.Name.Contains("Properties"))
                        metric.Properties.Add(prop.Name, prop.Value.ToString());

                list.Add(metric);
                return list;
            }

            return null;
        }

        private IEnumerable<ITelemetry> ConvertMetricTelemetry(string roleInstance, IReadOnlyDictionary<string, LogEventPropertyValue> Properties, string xCorrelationId)
        {
            //Get StructureValue
            if (TryGetStructureValue(Properties, METRICTELEMETRY, out StructureValue metricvalueStucValue) &&
                Throttle.Contains("Metric"))
            {
                var list = new List<ITelemetry>();
                //Parse values
                _ = double.TryParse(GetValueFromStructureValue(metricvalueStucValue, VALUE), out double metricValue);
                TimeSpan duration = TimeSpan.TryParse(GetValueFromStructureValue(metricvalueStucValue, DURATION), out duration) ? duration : TimeSpan.MinValue;
                DateTimeOffset startTime = DateTimeOffset.FromUnixTimeMilliseconds(GetDateTimeOffsetFromStructureValue(metricvalueStucValue, STARTTIME));
                DateTimeOffset endTime = startTime.AddMilliseconds(duration.TotalMilliseconds);

                //Create Metric Telemetry
                var metric = new MetricTelemetry
                {
                    Timestamp = startTime,
                    Name = GetValueFromStructureValue(metricvalueStucValue, NAME)
                    .Replace("\"", string.Empty).Trim(),
                    Sum = metricValue,
                };

                metric.Properties.Add(STARTTIME, startTime.ToString("MM/dd/yyyy hh:mm:ss.fff"));
                metric.Properties.Add(ENDTIME, endTime.ToString("MM/dd/yyyy hh:mm:ss.fff"));
                metric.Properties.Add("X-Correlation-ID", xCorrelationId);
                metric.Properties.Add("roleInstance", roleInstance); 
                
                foreach (var prop in metricvalueStucValue.Properties)
                    if (prop.Name.Contains("Properties"))
                        metric.Properties.Add(prop.Name, prop.Value.ToString());

                list.Add(metric);
                return list;
            }
            return null;
        }

        private IEnumerable<ITelemetry> ConvertTraceTelemetry(string roleInstance, IReadOnlyDictionary<string, LogEventPropertyValue> Properties, string xCorrelationId)
        {
            if (TryGetStructureValue(Properties, "TraceTelemetry", out StructureValue TraceStucValue) &&
                Throttle.Contains("Trace"))
            {
                var list = new List<ITelemetry>();
                var trace = new TraceTelemetry
                {
                    Timestamp = DateTime.UtcNow,
                    Message = GetValueFromStructureValue(TraceStucValue, "Message")
                };
                foreach (var prop in TraceStucValue.Properties)
                    if (prop.Name.Contains("Properties"))
                        trace.Properties.Add(prop.Name, prop.Value.ToString());
                trace.Properties.Add("X-Correlation-ID", xCorrelationId);
                trace.Properties.Add("roleInstance", roleInstance);
                list.Add(trace);
                return list;
            }

            return null;
        }

        private IEnumerable<ITelemetry> ConvertRequestTelemetry(string roleInstance, IReadOnlyDictionary<string, LogEventPropertyValue> Properties, string xCorrelationId)
        {
            var list = new List<ITelemetry>();
            //Get StructureValue
            if (TryGetStructureValue(Properties, REQUESTTELEMETRY, out StructureValue structureValue) &&
                Throttle.Contains("Request"))
            {
                //Parse values
                var strURL = GetValueFromStructureValue(structureValue, URL);
                TimeSpan duration = TimeSpan.TryParse(GetValueFromStructureValue(structureValue, DURATION), out duration) ? duration : TimeSpan.MinValue;
                DateTimeOffset startTime = DateTimeOffset.FromUnixTimeMilliseconds(GetDateTimeOffsetFromStructureValue(structureValue, STARTTIME));
                DateTimeOffset endTime = startTime.AddMilliseconds(duration.TotalMilliseconds);


                //Create Request Telemetry
                var req = new RequestTelemetry
                {
                    Name = GetValueFromStructureValue(structureValue, NAME),
                    Timestamp = startTime,
                    Duration = duration,
                    ResponseCode = GetValueFromStructureValue(structureValue, RESPONSECODE),
                    Url = new Uri(strURL)
                };
                req.Properties.Add(STARTTIME, startTime.ToString("MM/dd/yyyy hh:mm:ss.fff"));
                req.Properties.Add(ENDTIME, endTime.ToString("MM/dd/yyyy hh:mm:ss.fff"));
                req.Properties.Add("Verb", GetValueFromStructureValue(structureValue, HTTPMETHOD));
                req.Properties.Add("X-Correlation-ID", xCorrelationId);
                req.Properties.Add("roleInstance", roleInstance);

                foreach (var prop in structureValue.Properties)
                    if (prop.Name.Contains("Properties"))
                        req.Properties.Add(prop.Name, prop.Value.ToString());

                list.Add(req);
                return list;
            }

            return null;
        }

        //private LoggingLevelSwitch Level
        //{
        //    set { LevelSwitch = value; }
        //}


        public static bool TryGetStructureValue(StructureValue value, string prop, out StructureValue outValue)
        {
            try
            {
                outValue = (StructureValue)value.Properties.ToList().Where(p => p.Name == prop).FirstOrDefault().Value;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool TryGetStructureValue(IReadOnlyDictionary<string, LogEventPropertyValue> Properties,
            string key, out StructureValue StucValue)
        {
            try
            {
                var exceptionvalue =
                    (StructureValue)Properties.Where(x => x.Key == key).ToList().FirstOrDefault().Value;
                StucValue = exceptionvalue;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetValueFromStructureValue(StructureValue value, string propName)
        {
            try
            {
                return value.Properties.ToList().Where(p => p.Name == propName).FirstOrDefault().Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static long GetDateTimeOffsetFromStructureValue(StructureValue value, string propName)
        {
            try
            {
                LogEventProperty logProp = value.Properties.ToList().Where(p => p.Name == propName).FirstOrDefault();
                return logProp == null ? DateTimeOffset.Now.ToUnixTimeMilliseconds() : long.Parse(logProp.Value.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IDictionary<string, string> AddPropertiesToReqTel(StructureValue StructureValue)
        {
            IDictionary<string, string> props = new Dictionary<string, string>();
            try
            {
                var eventProp = StructureValue.Properties.ToList().Where(p => p.Name == "Properties").FirstOrDefault();
                var seqVals = (SequenceValue)eventProp.Value;
                var propvalues = seqVals.Elements.ToList();
                propvalues.ForEach(p =>
                {
                    var stucval = (StructureValue)p;
                    var arryProp = stucval.Properties.ToArray();
                    props.Add(arryProp[0].Value.ToString().Replace("\"", string.Empty).Trim(),
                        arryProp[1].Value.ToString().Replace("\"", string.Empty).Trim());
                });
                return props;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public static void ChangeLoggingLevel(int level)
        //{
        //    switch (level)
        //    {
        //        case 0:
        //            levelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Verbose;
        //            break;
        //        case 1:
        //            levelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Debug;
        //            break;
        //        case 2:
        //            levelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Information;
        //            break;
        //        case 3:
        //            levelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Warning;
        //            break;
        //        case 4:
        //            levelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Error;
        //            break;
        //        case 5:
        //            levelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Fatal;
        //            break;
        //        default:
        //            levelSwitch.MinimumLevel = Serilog.Events.LogEventLevel.Information;
        //            break;
        //    }
        //}

        public static string GetCloudInstanceName()
        {
            try
            {
                return Environment.MachineName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}