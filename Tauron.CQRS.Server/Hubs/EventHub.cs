using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Tauron.CQRS.Common.ServerHubs;

namespace Tauron.CQRS.Server.Hubs
{
    public class EventHub : Hub, IEventBus
    {
        public async Task Subscribe(string eventName) 
            => await Groups.AddToGroupAsync(Context.ConnectionId, eventName);

        public async Task UnSubscribe(string eventName) 
            => await Groups.RemoveFromGroupAsync(Context.ConnectionId, eventName);

        public async Task PublishEvent(DomainEvent @event, string apiKey)
        {

        }
    }
}