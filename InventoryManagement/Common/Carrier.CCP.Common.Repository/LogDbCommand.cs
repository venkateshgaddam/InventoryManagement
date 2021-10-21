using Carrier.logging.Service;
using Carrier.logging.Service.Models;
using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Carrier.CCP.Common.Repository
{
    /// <summary>
    /// 
    /// </summary>
    public class LogDbCommand : DbCommand
    {
        private readonly IPlatformLogger _logger;
        private DbCommand _command;
        private bool _disposed;

        /// <summary>
        /// 
        /// </summary>
        public override string CommandText
        {
            get => _command.CommandText;
            set => _command.CommandText = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public override int CommandTimeout
        {
            get => _command.CommandTimeout;
            set => _command.CommandTimeout = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public override CommandType CommandType
        {
            get => _command.CommandType;
            set => _command.CommandType = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public override UpdateRowSource UpdatedRowSource
        {
            get => _command.UpdatedRowSource;
            set => _command.UpdatedRowSource = value;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override DbConnection DbConnection
        {
            get => _command.Connection;
            set => _command.Connection = value;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override DbParameterCollection DbParameterCollection => _command.Parameters;

        /// <summary>
        /// 
        /// </summary>
        protected override DbTransaction DbTransaction
        {
            get => _command.Transaction;
            set => _command.Transaction = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool DesignTimeVisible
        {
            get => _command.DesignTimeVisible;
            set => _command.DesignTimeVisible = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="Command"></param>
        public LogDbCommand(IPlatformLogger logger, DbCommand Command)
        {
            _logger = logger;
            _command = Command;
        }

        /// <summary>
        /// 
        /// </summary>
        ~LogDbCommand() => Dispose(false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Disposing"></param>
        protected override void Dispose(bool Disposing)
        {
            if (_disposed) return;
            if (Disposing)
            {
                // No managed resources to release.
            }
            // Release unmanaged resources.
            _command?.Dispose();
            _command = null;
            // Do not release logger.  Its lifetime is controlled by caller.
            _disposed = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Cancel()
        {
            _command.Cancel();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int ExecuteNonQuery()
        {
            //LogCommandBeforeExecuted();

            Stopwatch stopwatch = Stopwatch.StartNew();
            long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            int result = _command.ExecuteNonQuery();

            var location = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()?.Name}";
            LogDependency(location, startTime, TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds));

            //LogCommandAfterExecuted();

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override object ExecuteScalar()
        {
            //LogCommandBeforeExecuted();

            Stopwatch stopwatch = Stopwatch.StartNew();
            long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            var result = _command.ExecuteScalar();

            var location = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()?.Name}";
            LogDependency(location, startTime, TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds));

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Prepare()
        {
            _command.Prepare();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override DbParameter CreateDbParameter() => _command.CreateParameter();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Behavior"></param>
        /// <returns></returns>
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior Behavior)
        {
            //LogCommandBeforeExecuted();

            Stopwatch stopwatch = Stopwatch.StartNew();
            long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            var result = _command.ExecuteReader(Behavior);

            var location = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()?.Name}";
            LogDependency(location, startTime, TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds));

            return result;
        }

        private string LogCommandBeforeExecuted()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Database command type = {_command.CommandType}");
            stringBuilder.AppendLine($"Database command text = {_command.CommandText}.");
            foreach (IDataParameter parameter in _command.Parameters)
            {
                if ((parameter.Direction == ParameterDirection.Output) || (parameter.Direction == ParameterDirection.ReturnValue))
                    continue;
                stringBuilder.AppendLine($"Database command parameter {parameter.ParameterName} = {parameter.Value}.");
            }
            return stringBuilder.ToString();
            //_logger.Log(_correlationId, stringBuilder.ToString());
        }

        private string LogCommandAfterExecuted()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (IDataParameter parameter in _command.Parameters)
            {
                if (parameter.Direction == ParameterDirection.Input)
                    continue;
                stringBuilder.AppendLine($"Database command parameter {parameter.ParameterName} = {parameter.Value}.");
            }
            return stringBuilder.ToString();
            //_logger.Log(_correlationId, stringBuilder.ToString());
        }

        private string LogCommand()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Type : {_command.CommandType}");
            stringBuilder.AppendLine($"Text : {_command.CommandText}.");
            foreach (IDataParameter parameter in _command.Parameters)
            {
                stringBuilder.AppendLine($"{parameter.ParameterName} , {parameter.Direction} : {parameter.Value}.");
            }
            return stringBuilder.ToString();
        }

        private void LogDependency(string location, long startTime, TimeSpan duration)
        {
            if (_logger == null)
                return;

            var dlog = new DependencyLog
            {
                Name = location,
                Duration = duration,
                StartTime = startTime
            };
            dlog.Properties.Add("Arguments", LogCommandBeforeExecuted());
            dlog.Properties.Add("Duration", duration.ToString());
            dlog.Properties.Add("ResponseResult", LogCommandAfterExecuted());
            _logger.InstanceLogger.Information("{@DependencyTelemetry}", dlog);
        }
    }
}
