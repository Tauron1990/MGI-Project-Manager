using System;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;

namespace Tauron.TestHelper.Mocks
{
    public sealed class MockEventPublisher : IEventPublisher
    {
        private readonly Action<object> _publish;

        public MockEventPublisher(Action<object> publish = null) 
            => _publish = publish;

        public Task Publish<T>(T @event, CancellationToken cancellationToken = new CancellationToken()) where T : class, IEvent
        {
            _publish?.Invoke(@event);

            return Task.CompletedTask;
        }
    }
}