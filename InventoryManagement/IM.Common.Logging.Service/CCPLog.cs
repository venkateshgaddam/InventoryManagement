using System;
//using Microsoft.WindowsAzure.Storage;
//using Microsoft.WindowsAzure.Storage.Table;

namespace IM.Common.Logging.Service
{
    public class CCPLog
    {
        public string Type { get; set; }

        public string User { get; set; }

        public string SessionID { get; set; }

        public string ConnectionID { get; set; }

        public string TSTAT { get; set; }

        public string Operation { get; set; }

        public string InputParam { get; set; }

        public string OutputParam { get; set; }

        //public DateTime Time { get; set; }

        public string Message { get; set; }

        public string Remark { get; set; }

        public string ServerType { get; set; }

        public string ActorType { get; set; }

        public string CorrelationId { get; set; }
    }

    public enum LogCategory
    {
        Information,
        Debug,
        Warning,
        Error,
        Critical,
        AuditLog,
        AuditLogError,
        EnergyLog
    }

    public enum CCPAction
    {
        Log,
        Alert,
        Notification
    }

    public enum ServerType
    {
        SignalRHub,
        Rest,
        Auth,
        RedisWorker,
        EnergyCalculation,
        ServiceBus,
        OpenAPI,
        PSUEventHubPublisher,
        PSUWorker,
        PredictiveAnalytics
    }

    public enum ActorType
    {
        External,
        Server
    }

    public class CCPLogException
    {
        public string MstrExternalMessageL1 { get; set; }
        public string MErrorCodeL1 { get; set; }
        public string MErrorCodeL2 { get; set; }
        public string MstrExternalMessageL2 { get; set; }
        public string MappingHttpErrorCode { get; set; }
        public string SystemIdentifier { get; set; }
        public string StackTrace { get; set; }
        public string Message { get; set; }
        public string TargetSite { get; set; }
        public string Source { get; set; }
        public Exception MInnerException { get; set; }
        public string CallerName { get; set; }

        //private string GetCallerName()
        //{
        //    try
        //    {
        //        return new StackFrame(3).GetMethod().Name;
        //    }
        //    catch (Exception)
        //    {
        //        return string.Empty;
        //    }
        //}

        //public SentinelLogException(SentinelException sentinelException)
        //{
        //    MstrExternalMessageL1 = sentinelException.mstrExternalMessageL1;
        //    MErrorCodeL1 = sentinelException.MErrorCodeL1;
        //    MErrorCodeL2 = sentinelException.MErrorCodeL2;
        //    MstrExternalMessageL2 = sentinelException.MstrExternalMessageL2;
        //    MappingHttpErrorCode = sentinelException.MappingHttpErrorCode;
        //    SystemIdentifier = sentinelException.SystemIdentifier;
        //    StackTrace = sentinelException.StackTrace;
        //    Message = sentinelException.Message;
        //    TargetSite = sentinelException.TargetSite;
        //    Source = sentinelException.Source;
        //    MInnerException = sentinelException.MInnerException;
        //    CallerName = GetCallerName();
        //}
    }

    /// <summary>
    ///     Class is created to just Make the replica of SentinelLog with Other name Called SentinelEventLog
    ///     In Future we can Exted for any specific property for this logging
    /// </summary>
    public class CCPEventLog : CCPLog
    {
        public string ClientId { get; set; }
    }
}