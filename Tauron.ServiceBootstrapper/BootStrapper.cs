using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tauron.CQRS.Common.Configuration;
using Tauron.CQRS.Services;
using Tauron.CQRS.Services.Extensions;
using Tauron.ServiceBootstrapper.Core;

namespace Tauron.ServiceBootstrapper
{
    [PublicAPI]
    public static class BootStrapper
    {
        private static readonly ManualResetEvent ExitWaiter = new ManualResetEvent(false);
        private static IServiceProvider _serviceProvider;

        public static async Task Run<TStartOptions>(
            string[] args, Action<ClientCofiguration> clientConfig = null,
            Func<ServiceCollection, Task> config = null, 
            Func<IServiceProvider, StartOptions<TStartOptions>, Task<bool>> startUp = null)
        {
            var rootPath = AppContext.BaseDirectory;
            var collection = new ServiceCollection();

            collection.AddLogging(lb =>
            {
                lb.AddConsole();
                lb.AddProvider(new SendingProvider(GetScopeFactory));
                lb.AddFile(o => o.BasePath = rootPath);
            });
            collection.AddOptions();
            var configuration = GetAppConfig(rootPath);
            if (config != null) 
                collection.AddSingleton(configuration);


            var serviceConfig = GetServiceConfig(rootPath);
            collection.AddCQRSServices(cofiguration =>
            {
                cofiguration.SetUrls(
                    new Uri(serviceConfig.GetValue<string>("Dispatcher"), UriKind.RelativeOrAbsolute),
                    serviceConfig.GetValue<string>("ServiceName"),
                    serviceConfig.GetValue<string>("ApiKey"))
                    .AddFrom<TStartOptions>()
                    .AddFrom<ServiceStoppedHandler>();
                
                clientConfig?.Invoke(cofiguration);
            });

            if(config != null)
                await config(collection);

            var provider = collection.BuildServiceProvider();
            await provider.StartCQRS();
            
            _serviceProvider = provider;
            if (startUp != null)
                await startUp(provider, new StartOptions<TStartOptions>(args));

            ExitWaiter.WaitOne();

            await _serviceProvider.GetRequiredService<IDispatcherClient>().Stop();
            ExitWaiter.Dispose();
            provider.Dispose();
        }

        private static IServiceScopeFactory GetScopeFactory() => _serviceProvider?.GetService<IServiceScopeFactory>();

        private static IConfiguration GetServiceConfig(string path)
        {
            string realPath = Path.Combine(path, "ServiceSettings.json");
            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile(realPath);

            return configBuilder.Build();
        }

        private static IConfiguration GetAppConfig(string path)
        {
            string realPath = Path.Combine(path, "AppSettings.json");
            if (!File.Exists(realPath)) return null;

            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile(realPath);

            return configBuilder.Build();
        }

        internal static void Shutdown() => ExitWaiter.Set();
    }
}
