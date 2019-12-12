using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Tauron.Application.Pipes.IO
{
    public abstract class PipeBase : IPipe
    {
        private readonly PipeStream _pipeStream;
        
        private Func<byte[], int, Task> _reader = (bytes, i) => Task.CompletedTask;
        private int _run = 1;
        private byte[] _readBuffer = new byte[4096];

        protected PipeBase(PipeStream pipeStream) 
            => _pipeStream = pipeStream;


        public event Func<(Exception Exception, bool OnReader), Task<bool>>? OnError;
        public bool CanRead => _pipeStream.CanRead;
        public bool CanWrite => _pipeStream.CanWrite;
        public async Task Init(Func<byte[], int, Task> readHandler)
        {
            _reader = readHandler;
            await Connect(_pipeStream);

            if(CanRead)
#pragma warning disable 4014
                Task.Run(BeginRead);
#pragma warning restore 4014
        }

        public async Task Write(ArraySegment<byte> data)
        {
            try
            {
                var array = data.ToArray();
                await _pipeStream.WriteAsync(BitConverter.GetBytes(array.Length));
                await _pipeStream.WriteAsync(array);
            }
            catch (Exception e)
            {
                if (OnError == null) throw ;
                if (!await OnError((e, false)))
                    throw ;
            }
        }

        private async void BeginRead()
        {
            while (_run == 1)
            {
                try
                {
                    var buffer = await TryRead(4);
                    if (buffer == null) continue;

                    var lenght = BitConverter.ToInt32(buffer, 0);
                    if (lenght == int.MinValue) break;

                    buffer = await TryRead(lenght);
                    if (buffer == null) continue;

                    await _reader(buffer, lenght);
                }
                catch (Exception e)
                {
                    if(OnError == null) continue;
                    if(!await OnError((e, true)))
                        break;
                }
            }
        }

        private async Task<byte[]?> TryRead(int lenght)
        {
            if(_readBuffer.Length < lenght)
                Array.Resize(ref _readBuffer, (int)(lenght * 1.2));

            var temp = await _pipeStream.ReadAsync(_readBuffer, 0, lenght);
            return temp == 0 ? null : _readBuffer;
        }

        public void Dispose()
        {
            Interlocked.Exchange(ref _run, 0);

            if(CanWrite)
                _pipeStream.Write(BitConverter.GetBytes(int.MinValue));
            _pipeStream.Dispose();
        }

        protected abstract Task Connect(PipeStream stream);
    }
}