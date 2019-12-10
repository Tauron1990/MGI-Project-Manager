using System;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Tauron.Application.Pipes
{
    [PublicAPI]
    public interface IPipeServer<TMessage> : IDisposable, IAsyncDisposable
    {
        event Func<(Exception Exception, bool OnReader), Task<bool>>? ReadErrorEvent;

        event Func<MessageRecivedEventArgs<TMessage>, Task>? MessageRecivedEvent;

        bool CanRead { get; }

        bool CanWrite { get; }

        Task Connect();

        Task SendMessage(TMessage msg);
    }
}