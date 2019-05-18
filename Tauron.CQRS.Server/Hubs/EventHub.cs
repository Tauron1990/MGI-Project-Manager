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

        public EventHub(IApiKeyStore keyStore, IEventManager eventManager)
        {
            _keyStore = keyStore;
            _eventManager = eventManager;
        }

        public async Task Subscribe(string eventName, string apiKey)
        {
            if (!await _keyStore.Validate(apiKey)) throw new HubException("Api Key Validation Failed");
            await Groups.AddToGroupAsync(Context.ConnectionId, eventName);
        }

        public async Task UnSubscribe(string eventName, string apiKey)
        {
            if (!await _keyStore.Validate(apiKey)) throw new HubException("Api Key Validation Failed");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, eventName);
        }

        [UsedImplicitly]
        public async Task PublishEvent(DomainEvent @event, string apiKey)
        {
            if (!await _keyStore.Validate(apiKey)) throw new HubException("Api Key Validation Failed");

            await _eventManager.ProvideEvent(@event, apiKey);
        }

        [UsedImplicitly]
        public async Task TryAccept(int sequenceNumber, string service, string apiKey)
        {
            if (!await _keyStore.Validate(apiKey)) throw new HubException("Api Key Validation Failed");

            await _eventManager.TryAccept(Context.ConnectionId, sequenceNumber, service);
        }
    }
}