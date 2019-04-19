// LzInWindow.cs

using System.IO;

namespace Tauron.MgiProjectManager.Data.Compressor._7zip.Compress.LZ
{
    public class InWindow
    {
        private uint _blockSize; // Size of Allocated memory block
        protected byte[] BufferBase; // pointer to buffer with data

        protected uint BufferOffset;
        private uint _keepSizeAfter; // how many BYTEs must be kept buffer after _pos
        private uint _keepSizeBefore; // how many BYTEs must be kept in buffer before _pos

        private uint _pointerToLastSafePosition;
        protected uint _pos; // offset (from _buffer) of curent byte
        private uint _posLimit; // offset (from _buffer) of first byte when new block reading must be done
        private Stream _stream;
        private bool _streamEndWasReached; // if (true) then _streamPos shows real end of stream
        protected uint StreamPos; // offset (from _buffer) of first not read byte from Stream

        private void MoveBlock()
        {
            var offset = BufferOffset + _pos - _keepSizeBefore;
            // we need one additional byte, since MovePos moves on 1 byte.
            if (offset > 0)
                offset--;

            var numBytes = BufferOffset + StreamPos - offset;

            // check negative offset ????
            for (uint i = 0; i < numBytes; i++)
                BufferBase[i] = BufferBase[offset + i];
            BufferOffset -= offset;
        }

        private void ReadBlock()
        {
            if (_streamEndWasReached)
                return;
            while (true)
            {
                var size = (int) (0 - BufferOffset + _blockSize - StreamPos);
                if (size == 0)
                    return;
                var numReadBytes = _stream.Read(BufferBase, (int) (BufferOffset + StreamPos), size);
                if (numReadBytes == 0)
                {
                    _posLimit = StreamPos;
                    var pointerToPostion = BufferOffset + _posLimit;
                    if (pointerToPostion > _pointerToLastSafePosition)
                        _posLimit = _pointerToLastSafePosition - BufferOffset;

                    _streamEndWasReached = true;
                    return;
                }

                StreamPos += (uint) numReadBytes;
                if (StreamPos >= _pos + _keepSizeAfter)
                    _posLimit = StreamPos - _keepSizeAfter;
            }
        }

        private void Free() 
            => BufferBase = null;

        protected void Create(uint keepSizeBefore, uint keepSizeAfter, uint keepSizeReserv)
        {
            _keepSizeBefore = keepSizeBefore;
            _keepSizeAfter = keepSizeAfter;
            var blockSize = keepSizeBefore + keepSizeAfter + keepSizeReserv;
            if (BufferBase == null || _blockSize != blockSize)
            {
                Free();
                _blockSize = blockSize;
                BufferBase = new byte[_blockSize];
            }

            _pointerToLastSafePosition = _blockSize - keepSizeAfter;
        }

        protected void SetStream(Stream stream) 
            => _stream = stream;

        protected void ReleaseStream() 
            => _stream = null;

        protected void Init()
        {
            BufferOffset = 0;
            _pos = 0;
            StreamPos = 0;
            _streamEndWasReached = false;
            ReadBlock();
        }

        protected void MovePos()
        {
            _pos++;
            if (_pos <= _posLimit) return;

            var pointerToPostion = BufferOffset + _pos;
            if (pointerToPostion > _pointerToLastSafePosition)
                MoveBlock();
            ReadBlock();
        }

        protected byte GetIndexByte(int index) 
            => BufferBase[BufferOffset + _pos + index];

        // index + limit have not to exceed _keepSizeAfter;
        protected uint GetMatchLen(int index, uint distance, uint limit)
        {
            if (_streamEndWasReached)
                if (_pos + index + limit > StreamPos)
                    limit = StreamPos - (uint) (_pos + index);
            distance++;
            // Byte *pby = _buffer + (size_t)_pos + index;
            var pby = BufferOffset + _pos + (uint) index;

            uint i;
            for (i = 0; i < limit && BufferBase[pby + i] == BufferBase[pby + i - distance]; i++)
            {}

            return i;
        }

        protected uint GetNumAvailableBytes() 
            => StreamPos - _pos;

        protected void ReduceOffsets(int subValue)
        {
            BufferOffset += (uint) subValue;
            _posLimit -= (uint) subValue;
            _pos -= (uint) subValue;
            StreamPos -= (uint) subValue;
        }
    }
}