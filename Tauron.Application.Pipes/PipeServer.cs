using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MessagePack;
using Tauron.Application.Pipes.IO;

namespace Tauron.Application.Pipes
{
    [PublicAPI]
    public sealed class PipeServer<TMessage> : IPipeServer<TMessage>
    {
        public static readonly PipeServer<TMessage> Empty = new PipeServer<TMessage>();

        private readonly IPipe _pipe;

        private PipeServer()
            : this(new FakePipe())
        {
        }

        public PipeServer(IPipe pipe) => _pipe = pipe;

        public void Dispose()
        {
            _pipe.Dispose();
        }

        public event Func<(Exception Exception, bool OnReader), Task<bool>>? ReadErrorEvent;
        public event Func<MessageRecivedEventArgs<TMessage>, Task>? MessageRecivedEvent;
        public bool CanRead => _pipe.CanRead;
        public bool CanWrite => _pipe.CanWrite;

        public async Task Connect()
        {
            _pipe.OnError += async error =>
                             {
                                 var temp = ReadErrorEvent;
                                 if (temp != null)
                                     return await temp(error);
                                 return true;
                             };

            await _pipe.Init(MessageRecived);
        }

        public async Task SendMessage(TMessage msg)
        {
            var data = MessagePackSerializer.Serialize(msg);
            await _pipe.Write(data);
        }

        private async Task MessageRecived(byte[] data, int lenght)
        {
            var msg = MessagePackSerializer.Deserialize<TMessage>(data[..lenght]);
            if (MessageRecivedEvent != null)
                await MessageRecivedEvent(new MessageRecivedEventArgs<TMessage>(msg));
        }

        private class FakePipe : IPipe
        {
            public void Dispose()
            {
            }

            public event Func<(Exception Exception, bool OnReader), Task<bool>>? OnError;
            public bool CanRead => false;
            public bool CanWrite => false;

            public Task Init(Func<byte[], int, Task> readHandler) => Task.FromException(new NotSupportedException());

            public Task Write(byte[] segment) => Task.FromException(new NotSupportedException());
        }
    }
}