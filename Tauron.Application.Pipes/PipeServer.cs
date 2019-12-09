using System;
using System.IO;
using System.Threading.Tasks;
using Tauron.Application.Pipes.IO;

namespace Tauron.Application.Pipes
{
    public sealed class PipeServer<TMessage> : IPipeServer<TMessage>
    {
        private readonly IPipe _pipe;

        public PipeServer(IPipe pipe) => _pipe = pipe;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public event EventHandler<ErrorEventArgs> ReadErrorEvent;
        public event EventHandler<MessageRecivedEventArgs<TMessage>> MessageRecivedEvent;
        public bool CanRead { get; }
        public bool CanWrite { get; }
        public Task Connect()
        {
            throw new NotImplementedException();
        }

        public Task SendMessage(TMessage msg)
        {
            throw new NotImplementedException();
        }
    }
}