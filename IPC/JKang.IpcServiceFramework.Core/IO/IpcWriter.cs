using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.IpcServiceFramework.IO
{
    public class IpcWriter : IDisposable
    {
        private readonly bool _leaveOpen;
        private readonly byte[] _lengthBuffer = new byte[4];
        private readonly IIpcMessageSerializer _serializer;
        private readonly Stream _stream;

        public IpcWriter(Stream stream, IIpcMessageSerializer serializer)
            : this(stream, serializer, false)
        {
        }

        public IpcWriter(Stream stream, IIpcMessageSerializer serializer, bool leaveOpen)
        {
            _stream = stream;
            _serializer = serializer;
            _leaveOpen = leaveOpen;
        }

        public async Task WriteAsync(IpcRequest request,
            CancellationToken cancellationToken = default)
        {
            var binary = _serializer.SerializeRequest(request);
            await WriteMessageAsync(binary, cancellationToken).ConfigureAwait(false);
        }

        public async Task WriteAsync(IpcResponse response,
            CancellationToken cancellationToken = default)
        {
            var binary = _serializer.SerializeResponse(response);
            await WriteMessageAsync(binary, cancellationToken).ConfigureAwait(false);
        }

        private async Task WriteMessageAsync(byte[] binary, CancellationToken cancellationToken)
        {
            var length = binary.Length;
            _lengthBuffer[0] = (byte) length;
            _lengthBuffer[1] = (byte) (length >> 8);
            _lengthBuffer[2] = (byte) (length >> 16);
            _lengthBuffer[3] = (byte) (length >> 24);

            await _stream.WriteAsync(_lengthBuffer, 0, _lengthBuffer.Length, cancellationToken).ConfigureAwait(false);
            await _stream.WriteAsync(binary, 0, binary.Length, cancellationToken).ConfigureAwait(false);
        }

        #region IDisposible

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
                if (!_leaveOpen)
                    _stream.Dispose();

            _disposed = true;
        }

        #endregion
    }
}