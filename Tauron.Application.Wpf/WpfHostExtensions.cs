using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Tauron.Application.Wpf;
using Tauron.Application.Wpf.AppCore;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting
{
    [PublicAPI]
    public static class WpfHostExtensions
    {
        public static IHostBuilder UseWpf<TMainWindow>(this IHostBuilder hostBuilder, Action<WpfConfiguration>? config = null)
            where TMainWindow : class, IMainWindow
        {
            hostBuilder.ConfigureServices(sc =>
            {
                IocReplacer.Create(sc);

                sc.TryAddTransient<IMainWindow, TMainWindow>();
                sc.Configure<HostOptions>(o => o.ShutdownTimeout = TimeSpan.FromMinutes(1));

                var wpf = new WpfConfiguration(sc);
                config?.Invoke(wpf);
                wpf.Build();
            });

            return hostBuilder;
        }

        public static IHostBuilder UseStartUp<TStartup>(this IHostBuilder config)
            where TStartup : WpfStartup
        {
            config.ConfigureServices((hbc, sc) =>
            {
                sc.TryAddTransient<WpfStartup, TStartup>();

                var s = Activator.CreateInstance(typeof(TStartup), hbc.Configuration) as TStartup;
                
                s?.ConfigureServices(sc);
            });

            return config;
        }

        public static IServiceCollection AddSplash<TWindow>(this IServiceCollection collection) where TWindow : System.Windows.Window, new()
        {
            return collection.AddTransient<ISplashScreen, SimpleSplashScreen<TWindow>>();
        }
    }
}