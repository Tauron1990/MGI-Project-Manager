using System;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Caching;
using CQRSlite.Commands;
using CQRSlite.Domain;
using CQRSlite.Events;
using CQRSlite.Queries;
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
        public static void AddCQRSServices(this IServiceCollection services, Action<ClientCofiguration> config)
        {
            var clientCofiguration = new ClientCofiguration();
            services.TryAddSingleton<IOptions<ClientCofiguration>>(new OptionsWrapper<ClientCofiguration>(clientCofiguration));
            config(clientCofiguration);

            //Dynamic TypeHandling for Serialization
            //services.AddCQRSTypeHandling();

            //Client for the Dispatcher
            services.TryAddSingleton<ISnapshotStore, SnapshotServerStore>();
            services.TryAddSingleton<IEventStore, ServerEventStore>();
            services.TryAddSingleton<IDispatcherClient, DispatcherClient>();
            services.TryAddTransient<IDispatcherApi, DispatcherApi>();
            services.TryAddSingleton<ICache, MemoryCache>();
            services.TryAddSingleton(typeof(GlobalEventHandler<>), typeof(GlobalEventHandler<>));
            
            //Service Delegates for cqrs lite
            services.TryAddScoped<ICommandSender, CommandSender>();
            services.TryAddScoped<IEventPublisher, EventPublisher>();
            services.TryAddSingleton<IHandlerManager, HandlerManager>();
            services.TryAddScoped<IQueryProcessor, QueryProcessor>();

            //Processing and Data Services
            services.TryAddSingleton(typeof(IPersistApi), 
                provider => new RestEase.RestClient(provider.GetRequiredService<IOptions<ClientCofiguration>>().Value.PersistenceApiUrl).For<IPersistApi>());

            services.AddScoped<ISession, CqrsSession>();
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

        public static async Task StartCQRS(this IServiceProvider provider, CancellationToken cancellationToken = default)
        {
            using var scope = provider.CreateScope();
            await scope.ServiceProvider.GetRequiredService<IHandlerManager>().Init(cancellationToken);
        }

        public static ClientCofiguration AddAwaiter<TMessage, TRespond>(this ClientCofiguration clientCofiguration, IServiceCollection serviceCollection) 
            where TMessage : class, ICommand where TRespond : IEvent
        {
            serviceCollection.AddTransient<AwaiterBase<TMessage, TRespond>, SimpleAwaiter<TMessage, TRespond>>();

            if (clientCofiguration.IsHandlerRegistrated<TRespond, GlobalEventHandler<TRespond>>()) return clientCofiguration;

            //serviceCollection.TryAddSingleton<GlobalEventHandler<TRespond>, GlobalEventHandler<TRespond>>();
            clientCofiguration.RegisterEventHandler<TRespond, GlobalEventHandler<TRespond>>();
            
            return clientCofiguration;
        }

        public static void AddFrom<TType>(this IServiceCollection serviceCollection, ClientCofiguration config) 
            => ScanFrom(config, serviceCollection, typeof(TType));

        public static ClientCofiguration ScanFrom<TType>(this ClientCofiguration config)
        {
            ScanFrom(config, null, typeof(TType));
         
            return config;
        }

        private static void ScanFrom(ClientCofiguration config, IServiceCollection serviceCollection, Type targetType)
        {
            var asm = targetType.Assembly;

            foreach (var type in asm.GetTypes())
            {
                if (!type.IsDefined(typeof(CQRSHandlerAttribute), false)) continue;

                foreach (var @interface in type.GetInterfaces())
                {
                    if (!@interface.IsGenericType) continue;

                    if (@interface.GetGenericTypeDefinition() == typeof(IReadModel<>))
                    {
                    }

                    if (@interface.GetGenericTypeDefinition() != typeof(ICancellableCommandHandler<>) && @interface.GetGenericTypeDefinition() != typeof(ICancellableEventHandler<>) &&
                        @interface.GetGenericTypeDefinition() != typeof(ICommandHandler<>)            && @interface.GetGenericTypeDefinition() != typeof(IEventHandler<>))
                        continue;

                    string name = @interface.GetGenericArguments()[0].Name;

                    //config.RegisterType(name, @interface.GenericTypeArguments[0]);
                    config.RegisterHandler(name, type);

                    serviceCollection?.AddTransient(@interface);
                }


            }
        }
    }
}