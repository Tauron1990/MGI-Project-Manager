using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.SignalR;
using Tauron.CQRS.Common.ServerHubs;
using Tauron.CQRS.Server.Core;

namespace Tauron.CQRS.Server.Hubs
{
    public class EventHub : Hub, IEventBus
    {
        private readonly IApiKeyStore _keyStore;
        private readonly IEventManager _eventManager;
        private readonly IConnectionManager _connectionManager;

        public EventHub(IApiKeyStore keyStore, IEventManager eventManager, IConnectionManager connectionManager)
        {
            _keyStore = keyStore;
            _eventManager = eventManager;
            _connectionManager = connectionManager;
        }

        public override async Task OnConnectedAsync() => await _connectionManager.AddConnection(Context.ConnectionId);

        public override async Task OnDisconnectedAsync(Exception exception) => await _connectionManager.RemoveConnection(Context.ConnectionId);

        public async Task Subscribe(string eventName, string apiKey)
        {
            if (!await _keyStore.Validate(apiKey)) throw new HubException("Api Key Validation Failed");
            
            await Groups.AddToGroupAsync(Context.ConnectionId, eventName);
            await _connectionManager.AddToGroup(Context.ConnectionId, eventName);
        }

        public async Task UnSubscribe(string eventName, string apiKey)
        {
            if (!await _keyStore.Validate(apiKey)) throw new HubException("Api Key Validation Failed");

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, eventName);
            await _connectionManager.RemoveFromGroup(Context.ConnectionId, eventName);
        }

        [UsedImplicitly]
        public async Task PublishEvent(DomainMessage message, string apiKey)
        {
            if (!await _keyStore.Validate(apiKey)) throw new HubException("Api Key Validation Failed");

            await _eventManager.ProvideEvent(Context.ConnectionId, message, apiKey);
        }

        [UsedImplicitly]
        public async Task TryAccept(int sequenceNumber, string service, string apiKey)
        {
            if (!await _keyStore.Validate(apiKey)) throw new HubException("Api Key Validation Failed");

            await _eventManager.TryAccept(Context.ConnectionId, sequenceNumber, service);
        }

        [UsedImplicitly]
        public async Task StillConnected() 
            => await _connectionManager.StillConnected(Context.ConnectionId);
    }
}