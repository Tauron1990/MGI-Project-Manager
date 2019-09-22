using System;
using System.Globalization;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PresentationTheme.Aero;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using ServiceManager.Core;
using Tauron.CQRS.Common.Configuration;
using Tauron.CQRS.Services.Extensions;

namespace ServiceManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public sealed class EventLogger : ILogEventSink
        {
            public event Action<LogEvent> Log;

            public void Emit(LogEvent logEvent) => OnLog(logEvent);

            private void OnLog(LogEvent obj) => Log?.Invoke(obj);
        }

        private static EventLogger Logger { get; } = new EventLogger();
        private static LogEntries LogEntries { get; } = new LogEntries();

        public static ClientCofiguration ClientCofiguration { get; private set; }

        public App()
        {
            Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = AeroTheme.ResourceUri
            });
        }


        public static IServiceProvider CreateServiceCollection()
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Verbose()
               .WriteTo.Sink(Logger)
               .CreateLogger();
            Logger.Log += LoggerOnLog;


            var collection = new ServiceCollection();
            
            collection.AddCQRSServices(c =>
                                       {
                                           ClientCofiguration = c;
                                           c.AddFrom<App>(collection);
                                       });
            collection.AddLogging(lb => lb.AddSerilog(dispose: true));

            collection.AddSingleton(LogEntries);
            collection.AddSingleton<MainWindowsModel, MainWindowsModel>();

            return collection.BuildServiceProvider();
        }

        private static void LoggerOnLog(LogEvent obj) 
            => LogEntries.AddLog("Service Manager", obj.RenderMessage(CultureInfo.CurrentCulture));
    }
}
