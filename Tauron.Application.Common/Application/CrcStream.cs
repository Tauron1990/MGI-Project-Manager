using System.IO;
using JetBrains.Annotations;

namespace Tauron.Application
{
    /// <summary>
    /// Encapsulates a <see cref="System.IO.Stream" /> to calculate the CRC32 checksum on-the-fly as data passes through.
    /// </summary>
    [PublicAPI]
    public class CrcStream : Stream
    {
        /// <summary>
        /// Encapsulate a <see cref="System.IO.Stream" />.
        /// </summary>
        /// <param name="stream">The stream to calculate the checksum for.</param>
        public CrcStream(Stream stream)
            => Stream = stream;

        /// <summary>
        /// Gets the underlying stream.
        /// </summary>
        public Stream Stream { get; }

        public override bool CanRead => Stream.CanRead;

        public override bool CanSeek => Stream.CanSeek;

        public override bool CanWrite => Stream.CanWrite;

        public override void Flush()
            => Stream.Flush();

        public override long Length => Stream.Length;

        public override long Position
        {
            get => Stream.Position;
            set => Stream.Position = value;
        }

        public override long Seek(long offset, SeekOrigin origin) => Stream.Seek(offset, origin);

        public override void SetLength(long value) => Stream.SetLength(value);

        public override int Read(byte[] buffer, int offset, int count)
        {
            count = Stream.Read(buffer, offset, count);
            _readCrc = CalculateCrc(_readCrc, buffer, offset, count);
            return count;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Stream.Write(buffer, offset, count);

            _writeCrc = CalculateCrc(_writeCrc, buffer, offset, count);
        }

        private uint CalculateCrc(uint crc, byte[] buffer, int offset, int count)
        {
            unchecked
            {
                for (int i = offset, end = offset + count; i < end; i++)
                    crc = (crc >> 8) ^ _table[(crc ^ buffer[i]) & 0xFF];
            }

            return crc;
        }

        private static uint[] _table = GenerateTable();

        private static uint[] GenerateTable()
        {
            unchecked
            {
                uint[] table = new uint[256];

                const uint poly = 0xEDB88320;
                for (uint i = 0; i < table.Length; i++)
                {
                    var crc = i;
                    for (var j = 8; j > 0; j--)
                    {
                        if ((crc & 1) == 1)
                            crc = (crc >> 1) ^ poly;
                        else
                            crc >>= 1;
                    }

                    table[i] = crc;
                }

                return table;
            }

        }

        private uint _readCrc = 0xFFFFFFFF;

        /// <summary>
        /// Gets the CRC checksum of the data that was read by the stream thus far.
        /// </summary>
        public uint ReadCrc => _readCrc ^ 0xFFFFFFFF;

        private uint _writeCrc = 0xFFFFFFFF;

        /// <summary>
        /// Gets the CRC checksum of the data that was written to the stream thus far.
        /// </summary>
        public uint WriteCrc => _writeCrc ^ 0xFFFFFFFF;

        /// <summary>
        /// Resets the read and write checksums.
        /// </summary>
        public void ResetChecksum()
        {
            _readCrc = 0xFFFFFFFF;
            _writeCrc = 0xFFFFFFFF;
        }

        protected override void Dispose(bool disposing)
        {
            Stream.Dispose();
            base.Dispose(disposing);
        }
    }
}
