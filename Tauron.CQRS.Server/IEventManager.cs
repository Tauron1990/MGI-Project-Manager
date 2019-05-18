using System.Collections.Concurrent;
using System.Threading.Tasks;
using Tauron.CQRS.Common.ServerHubs;
using Tauron.CQRS.Server.Hubs;

namespace Tauron.CQRS.Server
{
    public interface IEventManager
    {
        BlockingCollection<RecivedDomainEvent> Dispatcher { get; }

        Task<bool> DeliverEvent(RecivedDomainEvent @event);

        Task TryAccept(string connectionId, int sequenceNumber, string service);

        Task ProvideEvent(DomainEvent domainEvent, string apiKey);
    }
}