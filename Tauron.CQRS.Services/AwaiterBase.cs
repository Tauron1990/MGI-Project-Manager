using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Commands;
using CQRSlite.Events;
using CQRSlite.Messages;

namespace Tauron.CQRS.Services
{
    public abstract class AwaiterBase<TMessage, TRespond> : IEventHandler<TRespond> where TRespond : IEvent where TMessage : class, ICommand
    {
        private readonly ICommandSender _commandSender;

        protected AwaiterBase(ICommandSender commandSender) 
            => _commandSender = commandSender;

        public abstract TRespond Last { get; }

        protected abstract Task PrepareForWait();

        protected abstract Task HandleImpl(TRespond respond);

        protected abstract Task<(bool, TRespond)> WaitForEvent(int timeout);

        public async Task Send(TMessage msg, CancellationToken token = default) 
            => await _commandSender.Send(msg, token);

        public async Task<(bool, TRespond)> SendAndAwait(TMessage msg, int timeout = 30_000, CancellationToken token = default)
        {
            await PrepareForWait();
            await _commandSender.Send(msg, token);
            return await WaitForEvent(timeout);
        }

        Task IHandler<TRespond>.Handle(TRespond message) => HandleImpl(message);
    }
}