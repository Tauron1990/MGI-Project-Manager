using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Tauron.CQRS.Common.ServerHubs;
using Tauron.CQRS.Server.Hubs;

namespace Tauron.CQRS.Server.EventStore
{
    [UsedImplicitly]
    public class EventManager : IEventManager
    {
        private class EventCookie : IDisposable
        {
            private readonly IConnectionManager _connectionManager;
            private readonly HashSet<string> _recivedServices = new HashSet<string>();
            private readonly CountdownEvent _waitLock;

            public RecivedDomainEvent DomainEvent { get; }

            private DateTime CreationTime { get; } = DateTime.Now;

            public EventCookie(RecivedDomainEvent domainEvent, IConnectionManager connectionManager)
            {
                _connectionManager = connectionManager;
                DomainEvent = domainEvent;
                _waitLock = new CountdownEvent(connectionManager.GetCurrentClients(domainEvent.RealMessage.EventName));
                connectionManager.ClientAdded += ConnectionManagerOnClientAdded;
                connectionManager.ClientRemoved += ConnectionManagerOnClientRemoved;
            }

            private void ConnectionManagerOnClientRemoved(string obj)
            {
                if(obj != DomainEvent.RealMessage.EventName || _waitLock.CurrentCount == 0) return;

                _waitLock.Signal();
            }

            private void ConnectionManagerOnClientAdded(string obj)
            {
                if (obj != DomainEvent.RealMessage.EventName) return;

                _waitLock.AddCount();
            }

            public bool CanAccept(string service)
            {
                lock (_recivedServices)
                    return _recivedServices.Add(service);
            }

            public bool IsOld(DateTime now) 
                => CreationTime + TimeSpan.FromHours(20) < now;

            public bool WaitForResponse(int timeout) => _waitLock.Wait(timeout);

            public void Respond() 
                => _waitLock.Signal();

            public void Dispose()
            {
                _waitLock?.Dispose();
                _connectionManager.ClientRemoved -= ConnectionManagerOnClientRemoved;
                _connectionManager.ClientAdded -= ConnectionManagerOnClientAdded;
            }
        }

        private readonly IHubContext<EventHub> _eventHub;
        private readonly IConnectionManager _connectionManager;
        private readonly ILogger<IEventManager> _logger;
        private readonly ConcurrentDictionary<int, EventCookie> _eventCookies = new ConcurrentDictionary<int, EventCookie>();

        private int _dispatcherStopped;

        private bool DispatcherStopped
        {
            get => (Interlocked.CompareExchange(ref _dispatcherStopped, 1, 1) == 1);
            set
            {
                if (value) Interlocked.CompareExchange(ref _dispatcherStopped, 1, 0);
                else Interlocked.CompareExchange(ref _dispatcherStopped, 0, 1);
            }
        }

        public BlockingCollection<RecivedDomainEvent> Dispatcher { get; }

        public EventManager(IHubContext<EventHub> eventHub, IConnectionManager connectionManager, ILogger<IEventManager> logger)
        {
            _eventHub = eventHub;
            _connectionManager = connectionManager;
            _logger = logger;
            Dispatcher = new BlockingCollection<RecivedDomainEvent>();
        }

        public async Task<bool> DeliverEvent(RecivedDomainEvent @event, CancellationToken token)
        {
            _logger.LogInformation($"Deliver Event: {@event.RealMessage.EventName} -- {@event.RealMessage.EventType}");

            var entry = new EventCookie(@event, _connectionManager);
            if (!_eventCookies.TryAdd(@event.RealMessage.SequenceNumber, entry)) return false;
            
            switch (@event.RealMessage.EventType)
            {
                case EventType.Command:
                    var currentTime = DateTime.Now;
                    var outdated    = _eventCookies.Where(e => e.Value.IsOld(currentTime)).ToList();
                    foreach (var cookie in outdated)
                    {
                        if (_eventCookies.TryRemove(cookie.Key, out var cookie2)) cookie2.Dispose();
                    }

                    await _eventHub.Clients.Group(entry.DomainEvent.RealMessage.EventName).SendAsync(HubEventNames.PropagateEvent, entry.DomainEvent.RealMessage, cancellationToken: token);

                    return entry.WaitForResponse(100_000);
                case EventType.Query:
                case EventType.TransistentEvent:
                    await _eventHub.Clients.Groups(entry.DomainEvent.RealMessage.EventName).SendAsync(HubEventNames.PropagateEvent, entry.DomainEvent.RealMessage, cancellationToken: token);
                    return true;
                default:
                    return false;
            }
        }

        public async Task TryAccept(string connectionId, int sequenceNumber, string service)
        {
            _logger.LogInformation($"Try Accept: {connectionId} -- {service}");

            if (_eventCookies.TryGetValue(sequenceNumber, out var eventCookie))
            {
                switch (eventCookie.DomainEvent.RealMessage.EventType)
                {
                    case EventType.Command:
                        try
                        {
                            if (eventCookie.CanAccept(service))
                            {
                                await _eventHub.Clients.Client(connectionId).SendAsync(HubEventNames.AcceptedEvent);
                                _logger.LogInformation($"Accept: {connectionId} -- {service}");
                            }
                            else
                            {
                                await _eventHub.Clients.Client(connectionId).SendAsync(HubEventNames.RejectedEvent, HubEventNames.RejectionReasons.EventConsumed);
                                _logger.LogInformation($"Reject: {connectionId} -- {service}");
                            }
                        }
                        finally
                        {
                            eventCookie.Respond();
                        }

                        break;
                    case EventType.TransistentEvent:
                        await _eventHub.Clients.Client(connectionId).SendAsync(HubEventNames.AcceptedEvent);
                        break;
                    default:
                        eventCookie.Respond();
                        await _eventHub.Clients.Client(connectionId).SendAsync(HubEventNames.AcceptedEvent);
                        break;
                }
            }
            else
                await _eventHub.Clients.Client(connectionId).SendAsync(HubEventNames.RejectedEvent, HubEventNames.RejectionReasons.NoEvent);
        }

        public async Task ProvideEvent(string sender, DomainMessage domainMessage, string apiKey)
        {
            _logger.LogInformation($"Provide Event: {sender} -- {domainMessage.EventName}");

            if (DispatcherStopped)
            {
                switch (domainMessage.EventName)
                {
                    case HubEventNames.DispatcherCommand.StartDispatcher:
                    case HubEventNames.DispatcherCommand.StopDispatcher:
                        Dispatcher.Add(new RecivedDomainEvent(domainMessage, apiKey));
                        break;
                    default:
                        await _eventHub.Clients.Client(sender).SendAsync(HubEventNames.RejectedEvent, HubEventNames.RejectionReasons.DispatcherStoped);
                        return;
                }
            }

            Dispatcher.Add(new RecivedDomainEvent(domainMessage, apiKey));
        }

        public Task StopDispatching()
        {
            DispatcherStopped = true;
            return Task.CompletedTask;
        }

        public Task StartDispatching()
        {
            DispatcherStopped = false;
            return Task.CompletedTask;
        }
    }
}