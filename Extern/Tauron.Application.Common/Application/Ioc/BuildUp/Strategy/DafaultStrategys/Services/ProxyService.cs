﻿#region

using System;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using NLog;
using Tauron.Application.Ioc.BuildUp.Exports;
using ILogger = Castle.Core.Logging.ILogger;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The proxy service.</summary>
    public sealed class ProxyService : IProxyService
    {
        #region Constructors and Destructors

        public ProxyService()
        {
            GenericGenerator = new ProxyGenerator {Logger = new PrivateLogger()};
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the generator.</summary>
        /// <value>The generator.</value>
        public ProxyGenerator GenericGenerator { get; }

        #endregion

        public ProxyGenerator Generate(ExportMetadata metadata, ImportMetadata[] imports, out IImportInterceptor interceptor)
        {
            interceptor = null;

            return GenericGenerator;
        }

        /// <summary>The private logger.</summary>
        private class PrivateLogger : LevelFilteredLogger
        {
            #region Public Methods and Operators

            /// <summary>
            ///     The create child logger.
            /// </summary>
            /// <param name="loggerName">
            ///     The logger name.
            /// </param>
            /// <returns>
            ///     The <see cref="Castle.Core.Logging.ILogger" />.
            /// </returns>
            public override ILogger CreateChildLogger(string loggerName)
            {
                return new PrivateLogger();
            }

            #endregion

            #region Methods


            protected override void Log(LoggerLevel loggerLevel, string loggerName, string message, Exception exception)
            {
                LogLevel logLevel;

                switch (loggerLevel)
                {
                    case LoggerLevel.Debug:
                        logLevel = LogLevel.Debug;
                        break;
                    case LoggerLevel.Info:
                        logLevel = LogLevel.Info;
                        break;
                    case LoggerLevel.Warn:
                        logLevel = LogLevel.Warn;
                        break;
                    case LoggerLevel.Error:
                        logLevel = LogLevel.Error;
                        break;
                    case LoggerLevel.Fatal:
                        logLevel = LogLevel.Fatal;
                        break;
                    default:
                        logLevel = LogLevel.Trace;
                        break;
                }

                LogManager.GetLogger(loggerName, typeof(ProxyService)).Log(logLevel, exception, message);
            }

            #endregion
        }
    }
}