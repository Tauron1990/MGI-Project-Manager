using System;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Caching;
using CQRSlite.Commands;
using CQRSlite.Domain;
using CQRSlite.Domain.Exception;
using CQRSlite.Events;
using CQRSlite.Messages;
using CQRSlite.Queries;
using CQRSlite.Snapshotting;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Tauron.CQRS.Common.Configuration;
using Tauron.CQRS.Common.ServerHubs;
using Tauron.CQRS.Services.Core;
using Tauron.CQRS.Services.Core.Components;

namespace Tauron.CQRS.Services.Extensions
{
    [PublicAPI]
    public static class Extensions
    {
        private static readonly Random Random = new Random();

        public static ServerDomainMessage ToDomainMessage(this IMessage message, bool query = false)
        {
            var type = message.GetType();

            var msg = new ServerDomainMessage
            {
                EventData = JsonConvert.SerializeObject(message.Clear()),
                TypeName = type.AssemblyQualifiedName,
                EventName = query ? type.FullName : type.Name,
                EventType = EventType.Command,
                SequenceNumber = DateTime.UtcNow.Ticks + Random.Next()
            };

            switch (message)
            {
                case IEvent @event:
                    msg.Id = @event.Id;
                    msg.Version = @event.Version;
                    msg.TimeStamp = @event.TimeStamp;
                    msg.EventType = EventType.Event;
                    break;
                case IAmbientCommand _:
                    msg.EventType = EventType.AmbientCommand;
                    break;
            }

            return msg;
        }

        public static async Task<TAggregate> GetOrAdd<TAggregate>(this ISession session, Guid id)
            where TAggregate: CoreAggregateRoot, new()
        {
            try
            {
                return await session.Get<TAggregate>(id);
            }
            catch (AggregateNotFoundException)
            {
                var agg = new TAggregate();
                agg.SetId(id);
                await session.Add(agg);

                return agg;
            }
        }

        public static async Task<bool> Exis<TAggregate>(this ISession session, Guid id) 
            where TAggregate : AggregateRoot
        {
            try
            {
                await session.Get<TAggregate>(id);
                return true;
            }
            catch (AggregateNotFoundException)
            {
                return false;
            }
        }

        public static Task PublishEvent<T>(this ISession session, T @event, CancellationToken token = default)
            where T : class, IEvent
        {
            if(!(session is ICqrsSession internalSession)) 
                throw new InvalidOperationException("Only CqrsSessions are Compatible");

            return internalSession.EventPublisher.Publish(@event, token);
        }

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
            services.TryAddScoped(typeof(QueryAwaiter<>), typeof(QueryAwaiter<>));
            services.TryAddTransient(typeof(AwaiterBase<,>), typeof(SimpleAwaiter<,>));
            services.TryAddTransient<IAwaiterFactory, AwaiterFactory>();
            
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
                        new InternalRepository(store), store), store, s.GetRequiredService<ICache>());
            });
        }

        public static async Task StartCQRS(this IServiceProvider provider, CancellationToken cancellationToken = default)
        {
            using var scope = provider.CreateScope();
            await scope.ServiceProvider.GetRequiredService<IHandlerManager>().Init(cancellationToken);
        }

        public static ClientCofiguration AddAwaiter<TRespond>(this ClientCofiguration clientCofiguration) where TRespond : IEvent
        {
            //serviceCollection.AddTransient<AwaiterBase<TMessage, TRespond>, SimpleAwaiter<TMessage, TRespond>>();

            if (clientCofiguration.IsHandlerRegistrated<TRespond, GlobalEventHandler<TRespond>>()) return clientCofiguration;

            //serviceCollection.TryAddSingleton<GlobalEventHandler<TRespond>, GlobalEventHandler<TRespond>>();
            clientCofiguration.RegisterEventHandler<TRespond, GlobalEventHandler<TRespond>>();

            return clientCofiguration;
        }

        public static ClientCofiguration AddReadModel<TModel, TRespond, TQuery>(this ClientCofiguration configuration)
            where TModel : IReadModel<TRespond, TQuery> where TQuery : IQuery<TRespond>
        {
            AddReadModel(configuration, typeof(TModel), typeof(IReadModel<TRespond, TQuery>));

            return configuration;
        }

        public static void AddFrom<TType>(this IServiceCollection serviceCollection, ClientCofiguration config) 
            => ScanFrom(config, typeof(TType));

        public static ClientCofiguration AddFrom<TType>(this ClientCofiguration config)
        {
            ScanFrom(config, typeof(TType));
            return config;
        }

        public static ClientCofiguration AddType(this ClientCofiguration config, Type type)
        {
            foreach (var @interface in type.GetInterfaces())
            {
                if (!@interface.IsGenericType) continue;

                var genericDefinition = @interface.GetGenericTypeDefinition();

                if (genericDefinition == typeof(IReadModel<,>))
                {
                    AddReadModel(config, type, @interface);
                    continue;
                }

                if (genericDefinition != typeof(ICancellableCommandHandler<>) && genericDefinition != typeof(ICancellableEventHandler<>) &&
                    genericDefinition != typeof(ICommandHandler<>) && genericDefinition != typeof(IEventHandler<>) &&
                    genericDefinition != typeof(ISpecificationCommandHandler<>))
                    continue;

                var name = @interface.GetGenericArguments()[0].Name;

                config.RegisterHandler(name, type);
            }

            return config;
        }


        public static void AddReadModel(ClientCofiguration configuration, Type readModel, Type @interface) 
            => configuration.RegisterHandler(@interface.GetGenericArguments()[0].FullName, readModel);

        private static void ScanFrom(ClientCofiguration config, Type targetType)
        {
            var asm = targetType.Assembly;

            foreach (var type in asm.GetTypes())
            {
                if (!type.IsDefined(typeof(CQRSHandlerAttribute), false)) continue;

                config.AddType(type);
            }
        }
    }
}