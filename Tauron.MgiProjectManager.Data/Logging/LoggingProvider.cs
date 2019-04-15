using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Serilog.Events;
using Tauron.MgiProjectManager.Data.Contexts;
using Tauron.MgiProjectManager.Data.Models;

namespace Tauron.MgiProjectManager.Data.Logging
{
    public sealed class DatabaseLoggingProvider : ILogEventSink, IDisposable
    {
        private readonly Lazy<ILogger> _logger;
        private readonly List<LoggingEventEntity> _loggingEvents;
        private readonly object _lock = new object();

        public DatabaseLoggingProvider(ILoggerFactory loggerFactory)
        {
            _loggingEvents = new List<LoggingEventEntity>();
            _logger = new Lazy<ILogger>(() => loggerFactory.CreateLogger(nameof(DatabaseLoggingProvider)));
        }

        public void Emit(LogEvent logEvent)
        {
            lock (_lock)
            {
                _loggingEvents.Add(new LoggingEventEntity(logEvent));
                if (_loggingEvents.Count == 50) Task.Run(Start);
            }
        }

        private void Start()
        {
            LoggingEventEntity[] entities;
            lock (_lock)
            {
                entities = _loggingEvents.ToArray();
                _loggingEvents.Clear();
            }

            try
            {
                using var context = new LoggingDbContext();
                context.Events.AddRange(entities);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                try
                {
                    _logger.Value.LogError(e, "Error while Saving Log-Entitys");
                }
                catch
                {
                    // ignored
                }
            }
        }

        public void Dispose() 
            => Start();
    }
}