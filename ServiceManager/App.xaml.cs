using System;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using PresentationTheme.Aero;
using PresentationTheme.Aero.Win8;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using ServiceManager.ApiRequester;
using ServiceManager.Core;
using ServiceManager.Installation;
using ServiceManager.Installation.Core;
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
            Resources = new ResourceDictionary
            {
                Source = AeroWin8Theme.ResourceUri
            };
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

            collection.AddSingleton(Current.Dispatcher);
            collection.AddSingleton(LogEntries);
            collection.AddSingleton(provider => ServiceSettings.Read(MainWindowsModel.SettingsPath));

            collection.AddSingleton<MainWindowsModel, MainWindowsModel>();
            collection.AddTransient<IInstallerSystem, InstallerSystem>();
            collection.AddScoped<InstallerProcedure, InstallerProcedure>();

            collection.AddSingleton(provider
                => new RestEase.RestClient(
                    new Uri(
                        new Uri(provider.GetRequiredService<ServiceSettings>().Url), "Api/ApiRequester"))
                    .For<IApiRequester>());

            collection.AddSingleton(_ => (MainWindow)Current.MainWindow);
            collection.AddTransient(CreateControl<ApiControl>);
            collection.AddTransient(CreateControl<ApiWindow>);
            collection.AddTransient(CreateControl<ValueRequesterWindow>);
            collection.AddTransient(CreateControl<ServiceNameEnteringWindow>);
            collection.AddTransient(CreateControl<InstallerWindow>);

            return collection.BuildServiceProvider();
        }

        private static TType CreateControl<TType>(IServiceProvider provider)
            => provider.GetRequiredService<Dispatcher>().Invoke(() => ActivatorUtilities.CreateInstance<TType>(provider));

        private static void LoggerOnLog(LogEvent obj) 
            => LogEntries.AddLog("Service Manager", obj.RenderMessage(CultureInfo.CurrentCulture));
    }
}
