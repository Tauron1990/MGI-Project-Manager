using System;
using System.IO;
using System.Threading.Tasks;

namespace Tauron.Application.Pipes
{
    public sealed class AsyncMessanger<TMessage> : IPipeServer<TMessage>
    {
        private readonly IPipeServer<TMessage> _pipeServer;

        public AsyncMessanger(IPipeServer<TMessage> pipeServer) => _pipeServer = pipeServer;

        public event EventHandler<ErrorEventArgs> ReadErrorEvent
        {
            add => _pipeServer.ReadErrorEvent += value;
            remove => _pipeServer.ReadErrorEvent -= value;
        }

        public event EventHandler<MessageRecivedEventArgs<TMessage>> MessageRecivedEvent
        {
            add => _pipeServer.MessageRecivedEvent += value;
            remove => _pipeServer.MessageRecivedEvent -= value;
        }

        public bool CanRead => _pipeServer.CanRead;

        public bool CanWrite => _pipeServer.CanWrite;

        public Task Connect()
        {
            return _pipeServer.Connect();
        }

        public Task SendMessage(TMessage msg)
        {
            return _pipeServer.SendMessage(msg);
        }
    }
}