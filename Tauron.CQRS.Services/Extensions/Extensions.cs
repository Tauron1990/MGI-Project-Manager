using System;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Caching;
using CQRSlite.Commands;
using CQRSlite.Domain;
using CQRSlite.Events;
using CQRSlite.Snapshotting;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Tauron.CQRS.Common.Configuration;
using Tauron.CQRS.Services.Core;
using Tauron.CQRS.Services.Core.Components;

namespace Tauron.CQRS.Services.Extensions
{
    [PublicAPI]
    public static class Extensions
    {
        public static void AddCQRSServices(this IServiceCollection services)
        {
            //Dynamic TypeHandling for Serialization
            services.AddCQRSTypeHandling();

            //Client for the Dispatcher
            services.TryAddSingleton<ISnapshotStore, SnapshotServerStore>();
            services.TryAddSingleton<IEventStore, ServerEventStore>();
            services.TryAddSingleton<IDispatcherClient, DispatcherClient>();
            services.TryAddTransient<IDispatcherApi, DispatcherApi>();
            services.TryAddSingleton<IEventStore, ServerEventStore>();
            services.TryAddSingleton<ICache, MemoryCache>();
            
            //Service Delegates for cqrs lite
            services.TryAddScoped<ICommandSender, CommandSender>();
            services.TryAddScoped<IEventPublisher, EventPublisher>();
            services.TryAddSingleton<IHandlerManager, HandlerManager>();

            //Processing and Data Services
            services.TryAddSingleton(typeof(IPersistApi), 
                provider => new RestEase.RestClient(provider.GetRequiredService<IOptions<ClientCofiguration>>().Value.PersistenceApiUrl).For<IPersistApi>());

            services.AddScoped<ISession, Session>();
            services.AddScoped<IRepository>(s =>
            {
                var store = s.GetRequiredService<IEventStore>();
                var snapshot = s.GetRequiredService<ISnapshotStore>();

                return new CacheRepository(
                    new SnapshotRepository(
                        snapshot, s.GetService<ISnapshotStrategy>() ?? new DefaultSnapshotStrategy(),
                        new Repository(store), store), store, s.GetRequiredService<ICache>());
            });
        }

        public static async Task StartCQRS(this IServiceProvider provider, CancellationToken cancellationToken)
        {
            using (var scope = provider.CreateScope())
                await scope.ServiceProvider.GetRequiredService<IHandlerManager>().Init(cancellationToken);
        }

        public static ClientCofiguration ScanFrom<TType>(this ClientCofiguration config)
        {
            var asm = typeof(TType).Assembly;

            foreach (var type in asm.GetTypes())
            {
                if(!type.IsDefined(typeof(CQRSHandlerAttribute), false)) continue;

                foreach (var @interface in type.GetInterfaces())
                {
                    if(!@interface.IsGenericType) continue;

                    if (@interface.GetGenericTypeDefinition() != typeof(ICancellableCommandHandler<>) && @interface.GetGenericTypeDefinition() != typeof(ICancellableEventHandler<>) &&
                        @interface.GetGenericTypeDefinition() != typeof(ICommandHandler<>)            && @interface.GetGenericTypeDefinition() != typeof(IEventHandler<>))
                        continue;

                    config.RegisterType(@interface.GetGenericArguments()[0].Name, type);
                }
            }

            return config;
        }
    }
}