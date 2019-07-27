using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Commands;
using CQRSlite.Events;
using Nito.AsyncEx;

namespace Tauron.CQRS.Services.Core.Components
{
    public sealed class SimpleAwaiter<TMessage, TRespoand> : AwaiterBase<TMessage, TRespoand> where TRespoand : IEvent where TMessage : class, ICommand
    {
        private AsyncManualResetEvent _resetEvent;
        private TRespoand _last;
        private bool _finish; 

        public SimpleAwaiter(ICommandSender commandSender) : base(commandSender)
        {
        }

        public override TRespoand Last => _last;

        protected override Task PrepareForWait()
        {
            if(_resetEvent == null)
                _resetEvent = new AsyncManualResetEvent(true);

            _finish = false;
            _resetEvent.Reset();

            return Task.CompletedTask;
        }

        protected override Task HandleImpl(TRespoand respond)
        {
            _last = respond;
            _finish = true;

            _resetEvent?.Set();

            return Task.CompletedTask;
        }

        protected override async Task<(bool, TRespoand)> WaitForEvent(int timeout)
        {
            using var tokenSource = new CancellationTokenSource(timeout);
            await _resetEvent.WaitAsync(tokenSource.Token);

            return (_finish, Last);
        }
    }
}