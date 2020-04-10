using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
                sc.TryAddTransient<IMainWindow, TMainWindow>();

                IOCReplacer.Create(sc);
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
    }
}