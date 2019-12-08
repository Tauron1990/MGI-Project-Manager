using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Tauron.Application.Pipes.IO
{
    public abstract class PipeBase : IPipe
    {
        private readonly PipeStream _pipeStream;
        private readonly AsyncManualResetEvent _runLock = new AsyncManualResetEvent();

        private Func<byte[], int, Task> _reader = (bytes, i) => Task.CompletedTask;
        private int _run = 1;

        protected PipeBase(PipeStream pipeStream) 
            => _pipeStream = pipeStream;


        public event Func<(Exception Exception, bool OnReader), Task<bool>>? OnError;
        public bool CanRead => _pipeStream.CanRead;
        public bool CanWrite => _pipeStream.CanWrite;
        public Task Init(Func<byte[], int, Task> readHandler)
        {
            _reader = readHandler;

            if(CanRead)
                BeginRead();

            return Task.CompletedTask;
        }

        public async Task Write(byte[] data, int lenght)
        {
            await _pipeStream.WriteAsync(BitConverter.GetBytes(lenght));
            await _pipeStream.WriteAsync(data.AsMemory(..lenght));
        }

        private async void BeginRead()
        {
            while (_run == 1)
            {
                
            }

            _runLock.Set();
        }

        private async Task<byte[]> TryRead(int lenght)
        {
            _pipeStream.ReadAsync(, new CancellationToken())
        }

        public void Dispose()
        {
            Interlocked.Exchange(ref _run, 0);
            _runLock.Wait();

            _pipeStream.Write(BitConverter.GetBytes(int.MinValue));
            _pipeStream.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            Interlocked.Exchange(ref _run, 0);
            await _runLock.WaitAsync();

            _pipeStream.Write(BitConverter.GetBytes(int.MinValue));
            _pipeStream.Dispose();
        }
    }
}