using System;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MessagePack;
using Tauron.Application.Pipes.IO;

namespace Tauron.Application.Pipes
{
    [PublicAPI]
    public sealed class PipeServer<TMessage> : IPipeServer<TMessage>
    {
        private readonly IPipe _pipe;

        public PipeServer(IPipe pipe) => _pipe = pipe;

        public void Dispose() => _pipe.Dispose();

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

        private Task MessageRecived(byte[] arg1, int arg2)
        {
            
        }

        public async Task SendMessage(TMessage msg)
        {
            var data = MessagePackSerializer.SerializeUnsafe(msg);

            await _pipe.Write(data, data.Count);
        }

        public ValueTask DisposeAsync() 
            => _pipe.DisposeAsync();
    }
}