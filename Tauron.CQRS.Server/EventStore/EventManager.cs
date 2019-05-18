using System.Collections.Concurrent;
using System.Threading.Tasks;
using Tauron.CQRS.Common.ServerHubs;
using Tauron.CQRS.Server.Hubs;

namespace Tauron.CQRS.Server.EventStore
{
    public class EventManager : IEventManager
    {
        public BlockingCollection<RecivedDomainEvent> Dispatcher { get; }
        public Task<bool> DeliverEvent(RecivedDomainEvent @event)
        {
            
        }

        public Task TryAccept(string connectionId, int sequenceNumber, string service)
        {

        }

        public Task ProvideEvent(DomainEvent domainEvent, string apiKey)
        {

        }
    }
}