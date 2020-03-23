using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;
using Tauron.Application.Logging.impl;

namespace Tauron.Application.Logging
{
    [PublicAPI]
    public static class LoggingExtensions
    {
        public static LoggerConfiguration ConfigDefaultLogging(this LoggerConfiguration loggerConfiguration, string applicationName, Func<DocumentStore>? getStore = null, bool noFile = false)
        {
            if (getStore != null)
                loggerConfiguration.WriteTo.RavenDB(getStore(), expiration: TimeSpan.FromDays(100), errorExpiration: TimeSpan.FromDays(365));
            if(!noFile)
                loggerConfiguration.WriteTo.RollingFile(new CompactJsonFormatter(), "Logs\\Log.log", fileSizeLimitBytes: 5_242_880);

            return loggerConfiguration
                .MinimumLevel.Debug()
                //.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                //.MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.WithProperty("ApplicationName", applicationName)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithEventTypeEnricher();
        }

        public static LoggerConfiguration WithEventTypeEnricher(this LoggerEnrichmentConfiguration config) 
            => config.With<EventTypeEnricher>();

        public static IServiceCollection AddTauronLogging(this IServiceCollection collection)
        {
            collection.AddTransient(typeof(ISLogger<>), typeof(SeriLogger<>));

            return collection;
        }
    }
}
