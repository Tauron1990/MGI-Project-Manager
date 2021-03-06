﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using CQRSlite.Messages;
using CQRSlite.Queries;
using JetBrains.Annotations;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Tauron.CQRS.Common;
using Tauron.CQRS.Common.Configuration;
using Tauron.CQRS.Common.ServerHubs;
using Tauron.CQRS.Services.Core.Components;
using Tauron.CQRS.Services.Extensions;

namespace Tauron.CQRS.Services.Core
{
    [UsedImplicitly]
    public class DispatcherClient : ICoreDispatcherClient
    {
        private class DistapcherRetryPolicy : IRetryPolicy
        {
            public TimeSpan? NextRetryDelay(RetryContext retryContext)
            {
                switch (retryContext.PreviousRetryCount)
                {
                    case 1:
                        return TimeSpan.FromSeconds(1);
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        return TimeSpan.FromSeconds(2);
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                        return TimeSpan.FromSeconds(10);
                    default:
                        return TimeSpan.FromSeconds(30);
                }
            }
        }

        private class MessageDelivery
        {
            private readonly EventRegistration _eventRegistration;
            private readonly ServerDomainMessage _message;

            public MessageDelivery(EventRegistration eventRegistration, ServerDomainMessage message)
            {
                _eventRegistration = eventRegistration;
                _message = message;
            }

            public Task Start() => _eventRegistration.Process(_message);
        }

        private class EventRegistration
        {
            private readonly Func<IMessage, ServerDomainMessage, CancellationToken, Task> _msg;
            private readonly ILogger<IDispatcherClient> _logger;

            public EventRegistration(Func<IMessage, ServerDomainMessage, CancellationToken, Task> msg, ILogger<IDispatcherClient> logger)
            {
                _msg = msg;
                _logger = logger;
            }

            public async Task Process(ServerDomainMessage msg)
            {
                try
                {
                    await _msg.Invoke((IMessage) JsonConvert.DeserializeObject(msg.EventData, Type.GetType(msg.TypeName)), msg, CancellationToken.None);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error while Deligate to Messeage Handler {msg.EventName} {msg.EventType}");
                }
            }
        }

        private readonly IOptions<ClientCofiguration> _config;
        private readonly ILogger<IDispatcherClient> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly HubConnection _hubConnection;
        private readonly ConcurrentDictionary<string, EventRegistration> _eventRegistrations = new ConcurrentDictionary<string, EventRegistration>();
        private readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());
        private MessageQueue<MessageDelivery> _messageDeliveries;

        //private bool _isCLoseOk;

        public DispatcherClient(IOptions<ClientCofiguration> config, ILogger<IDispatcherClient> logger, IServiceScopeFactory scopeFactory)
        {
            _config = config;
            _logger = logger;
            _scopeFactory = scopeFactory;

            _hubConnection = new HubConnectionBuilder()
               .AddJsonProtocol()
               .WithUrl(config.Value.EventHubUrl)
               .WithAutomaticReconnect(new DistapcherRetryPolicy())
               .Build();
        }

        public async Task Start(CancellationToken token)
        {
            InvokeProcessor();
            await _hubConnection.StartAsync(token);

            //_hubConnection.Closed += HubConnectionOnClosed;
            _hubConnection.On(HubEventNames.PropagateEvent, new Action<ServerDomainMessage>(ProcessMessage));
            _hubConnection.On(HubEventNames.AcceptedEvent, new Action<long>(MessageAccept));
            _hubConnection.On(HubEventNames.RejectedEvent, new Action<string, int>(MessageReject));
        }

        public void AddHandler(string name, Func<IMessage, ServerDomainMessage, CancellationToken, Task> handler)
        {
            if(_eventRegistrations.ContainsKey(name)) return;

            _eventRegistrations[name] = new EventRegistration(handler, _logger);
        }

        private void InvokeProcessor()
        {
            Task.Run(async () =>
            {
                _messageDeliveries = new MessageQueue<MessageDelivery>(Environment.ProcessorCount);

                _messageDeliveries.OnWork += m => m.Start();
                _messageDeliveries.OnError += e =>
                {
                    _logger.LogError(e, "Error on Message Dispatch");
                    return Task.CompletedTask;
                };

                await _messageDeliveries.Start();

                _messageDeliveries.Dispose();
            });
        }

        public async Task Stop()
        {
            _messageDeliveries.Stop();
            //_isCLoseOk = true;
            await _hubConnection.StopAsync();
            await _hubConnection.DisposeAsync();
        }

        //private async Task HubConnectionOnClosed(Exception arg)
        //{
        //    //if(_isCLoseOk) return;

        //    //await _hubConnection.StartAsync();
        //}

        public async Task Send(IMessage command, CancellationToken cancellationToken)
        {
            //var msg = new ServerDomainMessage
            //                          {
            //                              EventData = JsonConvert.SerializeObject(command.Clear()),
            //                              TypeName = command.GetType().AssemblyQualifiedName,
            //                              EventName = command.GetType().Name,
            //                              EventType = EventType.Command,
            //                              SequenceNumber = DateTime.UtcNow.Ticks + _random.Next()
            //                          };

            //switch (command)
            //{
            //    case IEvent @event:
            //        msg.Id = @event.Id;
            //        msg.Version = @event.Version;
            //        msg.TimeStamp = @event.TimeStamp;
            //        msg.EventType = EventType.Event;
            //        break;
            //    case IAmbientCommand _:
            //        msg.EventType = EventType.AmbientCommand;
            //        break;
            //}

            using var source = new CancellationTokenSource(10_000);
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(source.Token, cancellationToken);

            await _hubConnection.SendAsync(HubEventNames.PublishEvent, command.ToDomainMessage(), _config.Value.ApiKey, linked.Token).ConfigureAwait(false);
        }

        public async Task SendToClient(string client, ServerDomainMessage serverDomainMessage, CancellationToken token) 
            => await _hubConnection.SendAsync(HubEventNames.PublishEventToClient, client, serverDomainMessage, _config.Value.ApiKey, token);

        public async Task SendEvents(IEnumerable<IEvent> events, CancellationToken cancellationToken)
        {
            await _hubConnection.SendAsync(HubEventNames.PublishEventGroup, events.Select(@event => @event.ToDomainMessage()).ToArray(), _config.Value.ApiKey, cancellationToken);
        }

        public async Task Subscribe(string name, Func<IMessage, ServerDomainMessage, CancellationToken, Task> msg)
        {
            try
            {
                var waiter = new EventRegistration(msg,_logger);
                _eventRegistrations[name] = waiter;

                await _hubConnection.InvokeAsync(HubEventNames.Subscribe, name, _config.Value.ApiKey);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Error on Subscribe Event");
                throw;
            }
        }

        public async Task<TResponse> Query<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var awaiter = scope.ServiceProvider.GetRequiredService<QueryAwaiter<TResponse>>();

            return await awaiter.SendQuery(query, cancellationToken, 
                async query1 => await _hubConnection.InvokeAsync(HubEventNames.PublishEvent, query.ToDomainMessage(true), _config.Value.ApiKey, cancellationToken)); 
        }

        private async void ProcessMessage(ServerDomainMessage domainMessage)
        {
            try
            {
                switch (domainMessage.EventType)
                {
                    case EventType.AmbientCommand:
                    case EventType.QueryResult:
                    case EventType.Event:
                    {
                        if (_eventRegistrations.TryGetValue(domainMessage.EventName, out var reg))
                            _messageDeliveries.Enqueue(new MessageDelivery(reg, domainMessage));
                        break;
                    }
                    default:
                        _memoryCache.Set(domainMessage.SequenceNumber, domainMessage);
                        await _hubConnection.SendAsync(HubEventNames.TryAccept, domainMessage.SequenceNumber, _config.Value.ServiceName, _config.Value.ApiKey);
                        break;
                }
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Error on Recieving Message");
            }
        }

        private void MessageReject(string reason, int seqNumber)
        {
            _memoryCache.Remove(seqNumber);
            _logger.LogInformation($"Command Rejected: {seqNumber} -- Reason: {reason}");
        }

        private void MessageAccept(long seqNumber)
        {
            try
            {
                var domainMessage = _memoryCache.Get<ServerDomainMessage>(seqNumber);
                if (domainMessage != null && _eventRegistrations.TryGetValue(domainMessage.EventName, out var reg))
                    _messageDeliveries.Enqueue(new MessageDelivery(reg, domainMessage));
                else
                    _logger.LogWarning($"Message Timeout Or Registration Error: {seqNumber}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error on Processing Message");
            }
        }
    }
}