using System;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using CQRSlite.Queries;
using Nito.AsyncEx;

namespace Tauron.CQRS.Services.Core.Components
{
    public sealed class QueryAwaiter<TRespond> : IEventHandler<QueryEvent<TRespond>>, IDisposable
    {
        private readonly IDisposable _disposable;
        private readonly ManualResetEvent _asyncManualReset;

        private TRespond _respond;

        public QueryAwaiter(GlobalEventHandler<QueryEvent<TRespond>> globalEventHandler)
        {
            _disposable = globalEventHandler.Register(this, t => Handle(t));
            _asyncManualReset = new ManualResetEvent(false);
        }


        public Task Handle(QueryEvent<TRespond> message)
        {
            if (message.EventName != typeof(TRespond).FullName) return Task.CompletedTask;

            _respond = message.Data;

            _asyncManualReset.Set();

            return Task.CompletedTask;
        }

        public async Task<TRespond> SendQuery(IQuery<TRespond> query, CancellationToken cancellationToken, Func<IQuery<TRespond>, Task> sender)
        {
            _asyncManualReset.Reset();

            await sender(query);
            _asyncManualReset.WaitOne(30_000);

            return _respond;
        }

        public void Dispose() => _disposable.Dispose();
    }
}