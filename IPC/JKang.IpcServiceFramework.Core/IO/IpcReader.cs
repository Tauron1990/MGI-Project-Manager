using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace JKang.IpcServiceFramework.IO
{
    public class IpcReader : IDisposable
    {
        private readonly bool _leaveOpen;
        private readonly byte[] _lengthBuffer = new byte[4];
        private readonly IIpcMessageSerializer _serializer;
        private readonly Stream _stream;

        public IpcReader(Stream stream, IIpcMessageSerializer serializer)
            : this(stream, serializer, false)
        {
        }

        public IpcReader(Stream stream, IIpcMessageSerializer serializer, bool leaveOpen)
        {
            _stream = stream;
            _serializer = serializer;
            _leaveOpen = leaveOpen;
        }

        public async Task<IpcRequest> ReadIpcRequestAsync(CancellationToken cancellationToken = default)
        {
            var binary = await ReadMessageAsync(cancellationToken).ConfigureAwait(false);
            return _serializer.DeserializeRequest(binary);
        }

        public async Task<IpcResponse> ReadIpcResponseAsync(CancellationToken cancellationToken = default)
        {
            var binary = await ReadMessageAsync(cancellationToken).ConfigureAwait(false);
            return _serializer.DeserializeResponse(binary);
        }

        private async Task<byte[]> ReadMessageAsync(CancellationToken cancellationToken)
        {
            var headerLength = await _stream.ReadAsync(_lengthBuffer, 0, _lengthBuffer.Length, cancellationToken);

            if (headerLength != 4) throw new ArgumentOutOfRangeException($"Header length must be 4 but was {headerLength}");

            var expectedLength = _lengthBuffer[0] | (_lengthBuffer[1] << 8) | (_lengthBuffer[2] << 16) | (_lengthBuffer[3] << 24);
            var bytes = new byte[expectedLength];
            var totalBytesReceived = 0;
            var remainingBytes = expectedLength;

            using (var ms = new MemoryStream())
            {
                while (totalBytesReceived < expectedLength)
                {
                    var dataLength = await _stream.ReadAsync(bytes, 0, remainingBytes, cancellationToken);

                    if (dataLength == 0) break; // end of stream or stream shut down.

                    await ms.WriteAsync(bytes, 0, dataLength, cancellationToken);
                    totalBytesReceived += dataLength;
                    remainingBytes -= dataLength;
                }

                bytes = ms.ToArray();
            }

            if (totalBytesReceived != expectedLength) throw new ArgumentOutOfRangeException($"Data length must be {expectedLength} but was {totalBytesReceived}");

            return bytes;
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
            {
                if (!_leaveOpen)
                    _stream.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}