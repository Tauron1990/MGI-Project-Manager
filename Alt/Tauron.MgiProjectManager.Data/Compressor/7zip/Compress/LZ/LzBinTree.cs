// LzBinTree.cs

using System;
using System.IO;
using Tauron.MgiProjectManager.Data.Compressor._7zip.Common;

namespace Tauron.MgiProjectManager.Data.Compressor._7zip.Compress.LZ
{
    public class BinTree : InWindow, IMatchFinder
    {
        private const uint kHash2Size = 1 << 10;
        private const uint kHash3Size = 1 << 16;
        private const uint Bt2HashSize = 1 << 16;
        private const uint kStartMaxLen = 1;
        private const uint kHash3Offset = kHash2Size;
        private const uint kEmptyHashValue = 0;
        private const uint kMaxValForNormalize = ((uint) 1 << 31) - 1;

        private uint _cutValue = 0xFF;
        private uint _cyclicBufferPos;
        private uint _cyclicBufferSize;
        private uint[] _hash;
        private uint _hashMask;
        private uint _hashSizeSum;
        private uint _matchMaxLen;

        private uint[] _son;

        private bool _hashArray = true;
        private uint _fixHashSize = kHash2Size + kHash3Size;
        private uint _minMatchCheck = 4;

        private uint _numHashDirectBytes;

        public new void SetStream(Stream stream)
        {
            base.SetStream(stream);
        }

        public new void ReleaseStream()
        {
            base.ReleaseStream();
        }

        public new void Init()
        {
            base.Init();
            for (uint i = 0; i < _hashSizeSum; i++)
                _hash[i] = kEmptyHashValue;
            _cyclicBufferPos = 0;
            ReduceOffsets(-1);
        }

        public new byte GetIndexByte(int index)
        {
            return base.GetIndexByte(index);
        }

        public new uint GetMatchLen(int index, uint distance, uint limit)
        {
            return base.GetMatchLen(index, distance, limit);
        }

        public new uint GetNumAvailableBytes()
        {
            return base.GetNumAvailableBytes();
        }

        public void Create(uint historySize, uint keepAddBufferBefore,
            uint matchMaxLen, uint keepAddBufferAfter)
        {
            if (historySize > kMaxValForNormalize - 256)
                throw new Exception();
            _cutValue = 16 + (matchMaxLen >> 1);

            var windowReservSize = (historySize + keepAddBufferBefore +
                                    matchMaxLen + keepAddBufferAfter) / 2 + 256;

            Create(historySize + keepAddBufferBefore, matchMaxLen + keepAddBufferAfter, windowReservSize);

            _matchMaxLen = matchMaxLen;

            var cyclicBufferSize = historySize + 1;
            if (_cyclicBufferSize != cyclicBufferSize)
                _son = new uint[(_cyclicBufferSize = cyclicBufferSize) * 2];

            var hs = Bt2HashSize;

            if (_hashArray)
            {
                hs = historySize - 1;
                hs |= hs >> 1;
                hs |= hs >> 2;
                hs |= hs >> 4;
                hs |= hs >> 8;
                hs >>= 1;
                hs |= 0xFFFF;
                if (hs > 1 << 24)
                    hs >>= 1;
                _hashMask = hs;
                hs++;
                hs += _fixHashSize;
            }

            if (hs != _hashSizeSum)
                _hash = new uint[_hashSizeSum = hs];
        }

        public uint GetMatches(uint[] distances)
        {
            uint lenLimit;
            if (_pos + _matchMaxLen <= StreamPos)
            {
                lenLimit = _matchMaxLen;
            }
            else
            {
                lenLimit = StreamPos - _pos;
                if (lenLimit < _minMatchCheck)
                {
                    MovePos();
                    return 0;
                }
            }

            uint offset = 0;
            var matchMinPos = _pos > _cyclicBufferSize ? _pos - _cyclicBufferSize : 0;
            var cur = BufferOffset + _pos;
            var maxLen = kStartMaxLen; // to avoid items for len < hashSize;
            uint hashValue, hash2Value = 0, hash3Value = 0;

            if (_hashArray)
            {
                var temp = CRC.Table[BufferBase[cur]] ^ BufferBase[cur + 1];
                hash2Value = temp & (kHash2Size - 1);
                temp ^= (uint) BufferBase[cur + 2] << 8;
                hash3Value = temp & (kHash3Size - 1);
                hashValue = (temp ^ (CRC.Table[BufferBase[cur + 3]] << 5)) & _hashMask;
            }
            else
            {
                hashValue = BufferBase[cur] ^ ((uint) BufferBase[cur + 1] << 8);
            }

            var curMatch = _hash[_fixHashSize + hashValue];
            if (_hashArray)
            {
                var curMatch2 = _hash[hash2Value];
                var curMatch3 = _hash[kHash3Offset + hash3Value];
                _hash[hash2Value] = _pos;
                _hash[kHash3Offset + hash3Value] = _pos;
                if (curMatch2 > matchMinPos)
                    if (BufferBase[BufferOffset + curMatch2] == BufferBase[cur])
                    {
                        distances[offset++] = maxLen = 2;
                        distances[offset++] = _pos - curMatch2 - 1;
                    }

                if (curMatch3 > matchMinPos)
                    if (BufferBase[BufferOffset + curMatch3] == BufferBase[cur])
                    {
                        if (curMatch3 == curMatch2)
                            offset -= 2;
                        distances[offset++] = maxLen = 3;
                        distances[offset++] = _pos - curMatch3 - 1;
                        curMatch2 = curMatch3;
                    }

                if (offset != 0 && curMatch2 == curMatch)
                {
                    offset -= 2;
                    maxLen = kStartMaxLen;
                }
            }

            _hash[_fixHashSize + hashValue] = _pos;

            var ptr0 = (_cyclicBufferPos << 1) + 1;
            var ptr1 = _cyclicBufferPos << 1;

            uint len1;
            var len0 = len1 = _numHashDirectBytes;

            if (_numHashDirectBytes != 0)
                if (curMatch > matchMinPos)
                    if (BufferBase[BufferOffset + curMatch + _numHashDirectBytes] !=
                        BufferBase[cur + _numHashDirectBytes])
                    {
                        distances[offset++] = maxLen = _numHashDirectBytes;
                        distances[offset++] = _pos - curMatch - 1;
                    }

            var count = _cutValue;

            while (true)
            {
                if (curMatch <= matchMinPos || count-- == 0)
                {
                    _son[ptr0] = _son[ptr1] = kEmptyHashValue;
                    break;
                }

                var delta = _pos - curMatch;
                var cyclicPos = (delta <= _cyclicBufferPos ? _cyclicBufferPos - delta : _cyclicBufferPos - delta + _cyclicBufferSize) << 1;

                var pby1 = BufferOffset + curMatch;
                var len = Math.Min(len0, len1);
                if (BufferBase[pby1 + len] == BufferBase[cur + len])
                {
                    while (++len != lenLimit)
                        if (BufferBase[pby1 + len] != BufferBase[cur + len])
                            break;
                    if (maxLen < len)
                    {
                        distances[offset++] = maxLen = len;
                        distances[offset++] = delta - 1;
                        if (len == lenLimit)
                        {
                            _son[ptr1] = _son[cyclicPos];
                            _son[ptr0] = _son[cyclicPos + 1];
                            break;
                        }
                    }
                }

                if (BufferBase[pby1 + len] < BufferBase[cur + len])
                {
                    _son[ptr1] = curMatch;
                    ptr1 = cyclicPos + 1;
                    curMatch = _son[ptr1];
                    len1 = len;
                }
                else
                {
                    _son[ptr0] = curMatch;
                    ptr0 = cyclicPos;
                    curMatch = _son[ptr0];
                    len0 = len;
                }
            }

            MovePos();
            return offset;
        }

        public void Skip(uint num)
        {
            do
            {
                uint lenLimit;
                if (_pos + _matchMaxLen <= StreamPos)
                {
                    lenLimit = _matchMaxLen;
                }
                else
                {
                    lenLimit = StreamPos - _pos;
                    if (lenLimit < _minMatchCheck)
                    {
                        MovePos();
                        continue;
                    }
                }

                var matchMinPos = _pos > _cyclicBufferSize ? _pos - _cyclicBufferSize : 0;
                var cur = BufferOffset + _pos;

                uint hashValue;

                if (_hashArray)
                {
                    var temp = CRC.Table[BufferBase[cur]] ^ BufferBase[cur + 1];
                    var hash2Value = temp & (kHash2Size - 1);
                    _hash[hash2Value] = _pos;
                    temp ^= (uint) BufferBase[cur + 2] << 8;
                    var hash3Value = temp & (kHash3Size - 1);
                    _hash[kHash3Offset + hash3Value] = _pos;
                    hashValue = (temp ^ (CRC.Table[BufferBase[cur + 3]] << 5)) & _hashMask;
                }
                else
                {
                    hashValue = BufferBase[cur] ^ ((uint) BufferBase[cur + 1] << 8);
                }

                var curMatch = _hash[_fixHashSize + hashValue];
                _hash[_fixHashSize + hashValue] = _pos;

                var ptr0 = (_cyclicBufferPos << 1) + 1;
                var ptr1 = _cyclicBufferPos << 1;

                uint len1;
                var len0 = len1 = _numHashDirectBytes;

                var count = _cutValue;
                while (true)
                {
                    if (curMatch <= matchMinPos || count-- == 0)
                    {
                        _son[ptr0] = _son[ptr1] = kEmptyHashValue;
                        break;
                    }

                    var delta = _pos - curMatch;
                    var cyclicPos = (delta <= _cyclicBufferPos ? _cyclicBufferPos - delta : _cyclicBufferPos - delta + _cyclicBufferSize) << 1;

                    var pby1 = BufferOffset + curMatch;
                    var len = Math.Min(len0, len1);
                    if (BufferBase[pby1 + len] == BufferBase[cur + len])
                    {
                        while (++len != lenLimit)
                            if (BufferBase[pby1 + len] != BufferBase[cur + len])
                                break;
                        if (len == lenLimit)
                        {
                            _son[ptr1] = _son[cyclicPos];
                            _son[ptr0] = _son[cyclicPos + 1];
                            break;
                        }
                    }

                    if (BufferBase[pby1 + len] < BufferBase[cur + len])
                    {
                        _son[ptr1] = curMatch;
                        ptr1 = cyclicPos + 1;
                        curMatch = _son[ptr1];
                        len1 = len;
                    }
                    else
                    {
                        _son[ptr0] = curMatch;
                        ptr0 = cyclicPos;
                        curMatch = _son[ptr0];
                        len0 = len;
                    }
                }

                MovePos();
            } while (--num != 0);
        }

        public void SetType(int numHashBytes)
        {
            _hashArray = numHashBytes > 2;
            if (_hashArray)
            {
                _numHashDirectBytes = 0;
                _minMatchCheck = 4;
                _fixHashSize = kHash2Size + kHash3Size;
            }
            else
            {
                _numHashDirectBytes = 2;
                _minMatchCheck = 2 + 1;
                _fixHashSize = 0;
            }
        }

        private new void MovePos()
        {
            if (++_cyclicBufferPos >= _cyclicBufferSize)
                _cyclicBufferPos = 0;
            base.MovePos();
            if (_pos == kMaxValForNormalize)
                Normalize();
        }

        private void NormalizeLinks(uint[] items, uint numItems, uint subValue)
        {
            for (uint i = 0; i < numItems; i++)
            {
                var value = items[i];
                if (value <= subValue)
                    value = kEmptyHashValue;
                else
                    value -= subValue;
                items[i] = value;
            }
        }

        private void Normalize()
        {
            var subValue = _pos - _cyclicBufferSize;
            NormalizeLinks(_son, _cyclicBufferSize * 2, subValue);
            NormalizeLinks(_hash, _hashSizeSum, subValue);
            ReduceOffsets((int) subValue);
        }
    }
}