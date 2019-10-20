using CQRSlite.Commands;
using CQRSlite.Events;

namespace Tauron.CQRS.Services
{
    public interface IAwaiterFactory
    {
        AwaiterBase<TMessage, TRespond> CreateAwaiter<TMessage, TRespond>()
            where TRespond : IEvent where TMessage : class, ICommand;
    }
}