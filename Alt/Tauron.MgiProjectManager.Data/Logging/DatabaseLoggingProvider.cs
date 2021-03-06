﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog.Core;
using Serilog.Events;
using Tauron.MgiProjectManager.Data.Contexts;
using Tauron.MgiProjectManager.Data.Models;

namespace Tauron.MgiProjectManager.Data.Logging
{
    public sealed class DatabaseLoggingProvider : ILogEventSink, IDisposable
    {
        private readonly object _lock = new object();
        private readonly Lazy<ILogger> _logger;
        private readonly List<LoggingEventEntity> _loggingEvents;
        private readonly IOptions<AppSettings> _settings;

        public DatabaseLoggingProvider(ILoggerFactory loggerFactory, IOptions<AppSettings> settings)
        {
            _settings = settings;
            _loggingEvents = new List<LoggingEventEntity>();
            _logger = new Lazy<ILogger>(() => loggerFactory.CreateLogger(nameof(DatabaseLoggingProvider)));
        }

        public void Dispose()
        {
            Start();
        }

        public void Emit(LogEvent logEvent)
        {
            lock (_lock)
            {
                _loggingEvents.Add(new LoggingEventEntity(logEvent));
                if (_loggingEvents.Count == _settings.Value.Logging.BatchEntries) Task.Run(Start);
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
    }
}