using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Tauron.CQRS.Common.ServerHubs;
using Tauron.CQRS.Server.Core;

namespace Tauron.CQRS.Server.Hubs
{
    public class EventHub : Hub, IEventBus
    {
        private readonly IApiKeyStore _keyStore;
        private readonly IEventManager _eventManager;
        private readonly IConnectionManager _connectionManager;
        private readonly ILogger<EventHub> _logger;

        public EventHub(IApiKeyStore keyStore, IEventManager eventManager, IConnectionManager connectionManager, ILogger<EventHub> logger)
        {
            _keyStore = keyStore;
            _eventManager = eventManager;
            _connectionManager = connectionManager;
            _logger = logger;
        }

        public override async Task OnConnectedAsync() => await _connectionManager.AddConnection(Context.ConnectionId);

        public override async Task OnDisconnectedAsync(Exception exception) => await _connectionManager.RemoveConnection(Context.ConnectionId);

        public async Task Subscribe(string eventName, string apiKey)
        {
            var vaildate = await _keyStore.Validate(apiKey);
            if (!vaildate.Ok) throw new HubException("Api Key Validation Failed");
            
            _logger.LogInformation($"Subscribe: {vaildate.ServiceName} -- {eventName}");
            await Groups.AddToGroupAsync(Context.ConnectionId, eventName);
            await _connectionManager.AddToGroup(Context.ConnectionId, eventName);
        }

        public async Task UnSubscribe(string eventName, string apiKey)
        {
            var vaildate = await _keyStore.Validate(apiKey);
            if (!vaildate.Ok) throw new HubException("Api Key Validation Failed");

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, eventName);
            await _connectionManager.RemoveFromGroup(Context.ConnectionId, eventName);
        }

        [UsedImplicitly]
        public async Task PublishEvent(ServerDomainMessage message, string apiKey)
        {
            var vaildate = await _keyStore.Validate(apiKey);
            if (!vaildate.Ok) throw new HubException("Api Key Validation Failed");

            _logger.LogInformation($"Event Publised: {vaildate.ServiceName} -- {message.EventName}");
            await _eventManager.ProvideEvent(Context.ConnectionId, message, apiKey);
        }

        [UsedImplicitly]
        public async Task PublishEventToClient(string client, ServerDomainMessage message, string apiKey)
        {
            var vaildate = await _keyStore.Validate(apiKey);
            if (!vaildate.Ok) throw new HubException("Api Key Validation Failed");

            await Clients.Client(client).SendAsync(HubEventNames.PropagateEvent, message);
        }

        [UsedImplicitly]
        public async Task PublishEventGroup(ServerDomainMessage[] messages, string apiKey)
        {
            var vaildate = await _keyStore.Validate(apiKey);
            if (!vaildate.Ok) throw new HubException("Api Key Validation Failed");

            _logger.LogInformation($"Event Publised: {vaildate.ServiceName}");

            foreach (var message in messages)
                await _eventManager.ProvideEvent(Context.ConnectionId, message, apiKey);
        }

        [UsedImplicitly]
        public async Task TryAccept(long sequenceNumber, string service, string apiKey)
        {
            var vaildate = await _keyStore.Validate(apiKey);
            if (!vaildate.Ok) throw new HubException("Api Key Validation Failed");

            await _eventManager.TryAccept(Context.ConnectionId, sequenceNumber, service);
        }

        [UsedImplicitly]
        public async Task StillConnected() 
            => await _connectionManager.StillConnected(Context.ConnectionId);
    }
}