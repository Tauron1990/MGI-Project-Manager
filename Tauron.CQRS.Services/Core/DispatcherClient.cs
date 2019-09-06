using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using CQRSlite.Queries;
using EasyNetQ;
using EasyNetQ.Consumer;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Tauron.CQRS.Common.Configuration;
using Tauron.CQRS.Common.ServerHubs;
using IMessage = CQRSlite.Messages.IMessage;

namespace Tauron.CQRS.Services.Core
{
    [UsedImplicitly]
    public class DispatcherClient : IDispatcherClient, IDisposable
    {
        private class EventRegistration
        {
            private readonly Func<IMessage, CancellationToken, Task> _msg;
            private readonly ILogger<IDispatcherClient> _logger;

            public EventRegistration(Func<IMessage, CancellationToken, Task> msg, ILogger<IDispatcherClient> logger)
            {
                _msg = msg;
                _logger = logger;
            }

            public async Task Process(DomainMessage msg)
            {
                try
                {
                    await _msg.Invoke((IMessage) JsonConvert.DeserializeObject(msg.EventData, Type.GetType(msg.TypeName)), CancellationToken.None);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error while Deligate to Messeage Handler {msg.EventName} {msg.EventType}");
                }
            }
        }

        private readonly Random _random = new Random();
        private readonly IOptions<ClientCofiguration> _config;
        private readonly ILogger<IDispatcherClient> _logger;
        private readonly ConcurrentDictionary<string, EventRegistration> _eventRegistrations = new ConcurrentDictionary<string, EventRegistration>();
        private readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());
        private readonly IBus _bus;

        public DispatcherClient(IOptions<ClientCofiguration> config, ILogger<IDispatcherClient> logger)
        {
            _config = config;
            _logger = logger;
            _bus = RabbitHutch.CreateBus($"host={config.Value.EventHubHost}", register => register.Register<IHandlerCollectionFactory, HandlerCollectionPerQueueFactory>());
            
        }
        
        public async Task Send(IMessage command, CancellationToken cancellationToken)
        {
            DomainMessage msg = new DomainMessage
                                      {
                                          EventData = JsonConvert.SerializeObject(command),
                                          TypeName = command.GetType().AssemblyQualifiedName,
                                          EventName = command.GetType().Name,
                                          EventType = EventType.Command,
                                          SequenceNumber = DateTime.UtcNow.Ticks + _random.Next()
                                      };

            if (command is IEvent @event)
            {
                msg.Id = @event.Id;
                msg.Version = @event.Version;
                msg.TimeStamp = @event.TimeStamp;
            }

            await _bus.PublishAsync(msg, configuration => configuration.WithQueueName(msg.EventName));
        }

        public Task Subsribe(string name, Func<IMessage, CancellationToken, Task> msg, bool isCommand)
        {
            try
            {
                var waiter = new EventRegistration(msg,_logger);
                _eventRegistrations[name] = waiter;

                _bus.SubscribeAsync<DomainMessage>(_config.Value.ServiceName, OnMessage, configuration =>
                {
                    configuration.WithQueueName(name).WithAutoDelete();
                    if (isCommand)
                        configuration.AsExclusive();
                });

                return Task.CompletedTask;
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Error on Subscribe Event");
                throw;
            }
        }

        private Task OnMessage(DomainMessage arg) => _eventRegistrations[arg.EventName].Process(arg);

        public Task<TResponse> Query<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
        {
            return _eventRegistrations
        }

        public void Dispose()
        {
            _memoryCache?.Dispose();
            _bus?.Dispose();
        }
    }
}