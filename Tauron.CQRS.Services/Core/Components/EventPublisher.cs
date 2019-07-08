using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;

namespace Tauron.CQRS.Services.Core.Components
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IDispatcherClient _dispatcher;

        public EventPublisher(IDispatcherClient dispatcher) => _dispatcher = dispatcher;

        public Task Publish<T>(T @event, CancellationToken cancellationToken = new CancellationToken()) where T : class, IEvent => _dispatcher.Send(@event, cancellationToken);
    }
}