using System;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Tauron.Application.Pipes
{
    [PublicAPI]
    public interface IPipeServer<TMessage>
    {
        event EventHandler<ErrorEventArgs> ReadErrorEvent;

        event EventHandler<MessageRecivedEventArgs<TMessage>> MessageRecivedEvent;

        bool CanRead { get; }

        bool CanWrite { get; }

        Task Connect();

        Task SendMessage(TMessage msg);
    }
}