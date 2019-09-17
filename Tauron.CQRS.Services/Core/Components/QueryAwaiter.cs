using System;
using System.Threading.Tasks;
using CQRSlite.Events;
using Nito.AsyncEx;

namespace Tauron.CQRS.Services.Core.Components
{
    public sealed class QueryAwaiter<TRespond> : IEventHandler<QueryEvent>, IDisposable 
        where TRespond : class, IEvent
    {
        private readonly IDispatcherClient _client;
        private readonly IDisposable _disposable;
        private readonly AsyncManualResetEvent _asyncManualReset;

        private TRespond _respond;

        public QueryAwaiter(GlobalEventHandler<QueryEvent> globalEventHandler, IDispatcherClient client)
        {
            _client = client;
            _disposable = globalEventHandler.Register(this, t => Handle(t));
            _asyncManualReset = new AsyncManualResetEvent(false);
        }


        public Task Handle(QueryEvent message)
        {
            if (message.EventName != typeof(TRespond).FullName) return Task.CompletedTask;

            _respond = message.Data?.ToObject<TRespond>();

            _asyncManualReset.Set();

            return Task.CompletedTask;
        }

        public void Dispose() => _disposable.Dispose();
    }
}