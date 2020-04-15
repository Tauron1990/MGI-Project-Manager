using System;
using System.Diagnostics;
using Catel.Logging;
using Microsoft.Extensions.Logging;

namespace Tauron.Application.Wpf.AppCore
{
    [DebuggerStepThrough]
    public sealed class CatelListner : LogListenerBase
    {
        private readonly ILogger<CatelListner> _logger;

        public CatelListner(ILogger<CatelListner> logger) 
            => _logger = logger;

        protected override void Write(ILog log, string message, LogEvent logEvent, object extraData, LogData logData, DateTime time)
        {
            //var level = logEvent switch
            //{
            //    LogEvent.Debug => LogLevel.Debug,
            //    LogEvent.Info => LogLevel.Information,
            //    LogEvent.Warning => LogLevel.Warning,
            //    LogEvent.Error => LogLevel.Error,
            //    LogEvent.Status => LogLevel.Trace,
            //    _ => throw new ArgumentOutOfRangeException()
            //};

            //string Formatter(object eventArgs, Exception exception) => FormatLogEvent(log, message, logEvent, extraData, logData, time);

            //_logger.Log(level, new EventId(-1, "Catel"), extraData, null, Formatter);
        }
    }
}