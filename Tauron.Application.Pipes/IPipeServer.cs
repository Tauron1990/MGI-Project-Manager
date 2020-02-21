using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Tauron.Application.Pipes
{
    [PublicAPI]
    public interface IPipeServer<TMessage> : IDisposable
    {
        bool CanRead { get; }

        bool CanWrite { get; }
        event Func<(Exception Exception, bool OnReader), Task<bool>>? ReadErrorEvent;

        event Func<MessageRecivedEventArgs<TMessage>, Task>? MessageRecivedEvent;

        Task Connect();

        Task SendMessage(TMessage msg);
    }
}