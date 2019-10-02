using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Tauron.CQRS.Common;
using Tauron.CQRS.Common.ServerHubs;
using Tauron.CQRS.Server.Hubs;

namespace Tauron.CQRS.Server
{
    public interface IEventManager
    {
        MessageQueue<RecivedDomainEvent> Dispatcher { get; }

        Task<bool> DeliverEvent(RecivedDomainEvent @event, CancellationToken token);

        Task TryAccept(string connectionId, long sequenceNumber, string service);

        Task ProvideEvent(string sender, ServerDomainMessage domainMessage, string apiKey);

        Task StopDispatching();

        Task StartDispatching();
    }
}