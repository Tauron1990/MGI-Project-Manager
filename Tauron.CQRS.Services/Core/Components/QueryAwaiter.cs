using System;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using CQRSlite.Queries;
using Nito.AsyncEx;

namespace Tauron.CQRS.Services.Core.Components
{
    public sealed class QueryAwaiter<TRespond> : IEventHandler<QueryEvent>, IDisposable
    {
        private readonly IDisposable _disposable;
        private readonly AsyncManualResetEvent _asyncManualReset;

        private TRespond _respond;

        public QueryAwaiter(GlobalEventHandler<QueryEvent> globalEventHandler)
        {
            _disposable = globalEventHandler.Register(this, t => Handle(t));
            _asyncManualReset = new AsyncManualResetEvent(false);
        }


        public Task Handle(QueryEvent message)
        {
            if (message.EventName != typeof(TRespond).FullName) return Task.CompletedTask;

            _respond = message.Data != null ? message.Data.ToObject<TRespond>() : default;

            _asyncManualReset.Set();

            return Task.CompletedTask;
        }

        public async Task<TRespond> SendQuery(IQuery<TRespond> query, CancellationToken cancellationToken, Func<IQuery<TRespond>, Task> sender)
        {
            var cancelSource = new CancellationTokenSource(30_000);
            var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancelSource.Token, cancellationToken);

            try
            {
                _asyncManualReset.Set();

                await sender(query);
                await _asyncManualReset.WaitAsync(linkedSource.Token);

                return _respond;
            }
            finally
            {
                cancelSource.Dispose();
                linkedSource.Dispose();
            }
        }

        public void Dispose() => _disposable.Dispose();
    }
}