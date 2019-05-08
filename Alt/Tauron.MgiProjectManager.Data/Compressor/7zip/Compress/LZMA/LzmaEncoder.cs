// LzmaEncoder.cs

using System;
using System.IO;
using Tauron.MgiProjectManager.Data.Compressor._7zip.Compress.LZ;
using Tauron.MgiProjectManager.Data.Compressor._7zip.Compress.RangeCoder;

namespace Tauron.MgiProjectManager.Data.Compressor._7zip.Compress.LZMA
{
    public class Encoder : ICoder, ISetCoderProperties, IWriteCoderProperties
    {
        private const uint kIfinityPrice = 0xFFFFFFF;

        private const int kDefaultDictionaryLogSize = 22;
        private const uint kNumFastBytesDefault = 0x20;

        private const uint kNumOpts = 1 << 12;

        private const int kPropSize = 5;

        private static readonly byte[] GFastPos = new byte[1 << 11];


        private static readonly string[] MatchFinderIDs =
        {
            "BT2",
            "BT4"
        };

        private uint _additionalOffset;
        private uint _alignPriceCount;
        private readonly uint[] _alignPrices = new uint[Base.AlignTableSize];

        private uint _dictionarySize = 1 << kDefaultDictionaryLogSize;
        private uint _dictionarySizePrev = 0xFFFFFFFF;
        private readonly uint[] _distancesPrices = new uint[Base.NumFullDistances << Base.NumLenToPosStatesBits];

        private uint _distTableSize = kDefaultDictionaryLogSize * 2;
        private bool _finished;
        private Stream _inStream;

        private readonly BitEncoder[] _isMatch = new BitEncoder[Base.NumStates << Base.NumPosStatesBitsMax];
        private readonly BitEncoder[] _isRep = new BitEncoder[Base.NumStates];
        private readonly BitEncoder[] _isRep0Long = new BitEncoder[Base.NumStates << Base.NumPosStatesBitsMax];
        private readonly BitEncoder[] _isRepG0 = new BitEncoder[Base.NumStates];
        private readonly BitEncoder[] _isRepG1 = new BitEncoder[Base.NumStates];
        private readonly BitEncoder[] _isRepG2 = new BitEncoder[Base.NumStates];

        private readonly LenPriceTableEncoder _lenEncoder = new LenPriceTableEncoder();

        private readonly LiteralEncoder _literalEncoder = new LiteralEncoder();
        private uint _longestMatchLength;

        private bool _longestMatchWasFound;

        private readonly uint[] _matchDistances = new uint[Base.MatchMaxLen * 2 + 2];
        private IMatchFinder _matchFinder;

        private EMatchFinderType _matchFinderType = EMatchFinderType.Bt4;
        private uint _matchPriceCount;

        private bool _needReleaseMfStream;
        private uint _numDistancePairs;

        private uint _numFastBytes = kNumFastBytesDefault;
        private uint _numFastBytesPrev = 0xFFFFFFFF;
        private int _numLiteralContextBits = 3;
        private int _numLiteralPosStateBits;
        private readonly Optimal[] _optimum = new Optimal[kNumOpts];
        private uint _optimumCurrentIndex;

        private uint _optimumEndIndex;
        private BitTreeEncoder _posAlignEncoder = new BitTreeEncoder(Base.NumAlignBits);

        private readonly BitEncoder[] _posEncoders = new BitEncoder[Base.NumFullDistances - Base.EndPosModelIndex];

        private readonly BitTreeEncoder[] _posSlotEncoder = new BitTreeEncoder[Base.NumLenToPosStates];

        private readonly uint[] _posSlotPrices = new uint[1 << (Base.NumPosSlotBits + Base.NumLenToPosStatesBits)];

        private int _posStateBits = 2;
        private uint _posStateMask = 4 - 1;
        private byte _previousByte;
        private readonly RangeCoder.Encoder _rangeEncoder = new RangeCoder.Encoder();
        private readonly uint[] _repDistances = new uint[Base.NumRepDistances];
        private readonly LenPriceTableEncoder _repMatchLenEncoder = new LenPriceTableEncoder();

        private Base.State _state = new Base.State();

        private uint _trainSize;
        private bool _writeEndMark;

        private long _nowPos64;
        private readonly byte[] _properties = new byte[kPropSize];
        private readonly uint[] _repLens = new uint[Base.NumRepDistances];

        private readonly uint[] _reps = new uint[Base.NumRepDistances];

        private readonly uint[] _tempPrices = new uint[Base.NumFullDistances];

        static Encoder()
        {
            const byte fastSlots = 22;
            var c = 2;
            GFastPos[0] = 0;
            GFastPos[1] = 1;
            for (byte slotFast = 2; slotFast < fastSlots; slotFast++)
            {
                var k = (uint) 1 << ((slotFast >> 1) - 1);
                for (uint j = 0; j < k; j++, c++)
                    GFastPos[c] = slotFast;
            }
        }

        public Encoder()
        {
            for (var i = 0; i < kNumOpts; i++)
                _optimum[i] = new Optimal();
            for (var i = 0; i < Base.NumLenToPosStates; i++)
                _posSlotEncoder[i] = new BitTreeEncoder(Base.NumPosSlotBits);
        }


        public void Code(Stream inStream, Stream outStream,
            long inSize, long outSize, ICodeProgress progress)
        {
            _needReleaseMfStream = false;
            try
            {
                SetStreams(inStream, outStream);
                while (true)
                {
                    CodeOneBlock(out var processedInSize, out var processedOutSize, out var finished);
                    if (finished)
                        return;
                    progress?.SetProgress(processedInSize, processedOutSize);
                }
            }
            finally
            {
                ReleaseStreams();
            }
        }

        public void SetCoderProperties(CoderPropID[] propIDs, object[] properties)
        {
            for (uint i = 0; i < properties.Length; i++)
            {
                var prop = properties[i];
                switch (propIDs[i])
                {
                    case CoderPropID.NumFastBytes:
                    {
                        if (!(prop is int))
                            throw new InvalidParamException();
                        var numFastBytes = (int) prop;
                        if (numFastBytes < 5 || numFastBytes > Base.MatchMaxLen)
                            throw new InvalidParamException();
                        _numFastBytes = (uint) numFastBytes;
                        break;
                    }

                    case CoderPropID.Algorithm:
                    {
                        /*
                        if (!(prop is Int32))
                            throw new InvalidParamException();
                        Int32 maximize = (Int32)prop;
                        _fastMode = (maximize == 0);
                        _maxMode = (maximize >= 2);
                        */
                        break;
                    }

                    case CoderPropID.MatchFinder:
                    {
                        if (!(prop is string))
                            throw new InvalidParamException();
                        var matchFinderIndexPrev = _matchFinderType;
                        var m = FindMatchFinder(((string) prop).ToUpper());
                        if (m < 0)
                            throw new InvalidParamException();
                        _matchFinderType = (EMatchFinderType) m;
                        if (_matchFinder != null && matchFinderIndexPrev != _matchFinderType)
                        {
                            _dictionarySizePrev = 0xFFFFFFFF;
                            _matchFinder = null;
                        }

                        break;
                    }

                    case CoderPropID.DictionarySize:
                    {
                        const int dicLogSizeMaxCompress = 30;
                        if (!(prop is int))
                            throw new InvalidParamException();

                        var dictionarySize = (int) prop;
                        if (dictionarySize < (uint) (1 << Base.DicLogSizeMin) ||
                            dictionarySize > (uint) (1 << dicLogSizeMaxCompress))
                            throw new InvalidParamException();
                        _dictionarySize = (uint) dictionarySize;
                        int dicLogSize;
                        for (dicLogSize = 0; dicLogSize < (uint) dicLogSizeMaxCompress; dicLogSize++)
                            if (dictionarySize <= (uint) 1 << dicLogSize)
                                break;
                        _distTableSize = (uint) dicLogSize * 2;
                        break;
                    }

                    case CoderPropID.PosStateBits:
                    {
                        if (!(prop is int))
                            throw new InvalidParamException();
                        var v = (int) prop;
                        if (v < 0 || v > (uint) Base.NumPosStatesBitsEncodingMax)
                            throw new InvalidParamException();
                        _posStateBits = v;
                        _posStateMask = ((uint) 1 << _posStateBits) - 1;
                        break;
                    }

                    case CoderPropID.LitPosBits:
                    {
                        if (!(prop is int))
                            throw new InvalidParamException();
                        var v = (int) prop;
                        if (v < 0 || v > Base.NumLitPosStatesBitsEncodingMax)
                            throw new InvalidParamException();
                        _numLiteralPosStateBits = v;
                        break;
                    }

                    case CoderPropID.LitContextBits:
                    {
                        if (!(prop is int))
                            throw new InvalidParamException();
                        var v = (int) prop;
                        if (v < 0 || v > Base.NumLitContextBitsMax)
                            throw new InvalidParamException();
                        
                        _numLiteralContextBits = v;
                        break;
                    }

                    case CoderPropID.EndMarker:
                    {
                        if (!(prop is bool))
                            throw new InvalidParamException();
                        SetWriteEndMarkerMode((bool) prop);
                        break;
                    }

                    default:
                        throw new InvalidParamException();
                }
            }
        }

        public void WriteCoderProperties(Stream outStream)
        {
            _properties[0] = (byte) ((_posStateBits * 5 + _numLiteralPosStateBits) * 9 + _numLiteralContextBits);
            for (var i = 0; i < 4; i++)
                _properties[1 + i] = (byte) ((_dictionarySize >> (8 * i)) & 0xFF);
            outStream.Write(_properties, 0, kPropSize);
        }

        private static uint GetPosSlot(uint pos)
        {
            if (pos < 1 << 11)
                return GFastPos[pos];
            if (pos < 1 << 21)
                return (uint) (GFastPos[pos >> 10] + 20);
            return (uint) (GFastPos[pos >> 20] + 40);
        }

        private static uint GetPosSlot2(uint pos)
        {
            if (pos < 1 << 17)
                return (uint) (GFastPos[pos >> 6] + 12);
            if (pos < 1 << 27)
                return (uint) (GFastPos[pos >> 16] + 32);
            return (uint) (GFastPos[pos >> 26] + 52);
        }

        private void BaseInit()
        {
            _state.Init();
            _previousByte = 0;
            for (uint i = 0; i < Base.NumRepDistances; i++)
                _repDistances[i] = 0;
        }

        private void Create()
        {
            if (_matchFinder == null)
            {
                var bt = new BinTree();
                var numHashBytes = 4;
                if (_matchFinderType == EMatchFinderType.Bt2)
                    numHashBytes = 2;
                bt.SetType(numHashBytes);
                _matchFinder = bt;
            }

            _literalEncoder.Create(_numLiteralPosStateBits, _numLiteralContextBits);

            if (_dictionarySize == _dictionarySizePrev && _numFastBytesPrev == _numFastBytes)
                return;
            _matchFinder.Create(_dictionarySize, kNumOpts, _numFastBytes, Base.MatchMaxLen + 1);
            _dictionarySizePrev = _dictionarySize;
            _numFastBytesPrev = _numFastBytes;
        }

        private void SetWriteEndMarkerMode(bool writeEndMarker) 
            => _writeEndMark = writeEndMarker;

        private void Init()
        {
            BaseInit();
            _rangeEncoder.Init();

            uint i;
            for (i = 0; i < Base.NumStates; i++)
            {
                for (uint j = 0; j <= _posStateMask; j++)
                {
                    var complexState = (i << Base.NumPosStatesBitsMax) + j;
                    _isMatch[complexState].Init();
                    _isRep0Long[complexState].Init();
                }

                _isRep[i].Init();
                _isRepG0[i].Init();
                _isRepG1[i].Init();
                _isRepG2[i].Init();
            }

            _literalEncoder.Init();
            for (i = 0; i < Base.NumLenToPosStates; i++)
                _posSlotEncoder[i].Init();
            for (i = 0; i < Base.NumFullDistances - Base.EndPosModelIndex; i++)
                _posEncoders[i].Init();

            _lenEncoder.Init((uint) 1 << _posStateBits);
            _repMatchLenEncoder.Init((uint) 1 << _posStateBits);

            _posAlignEncoder.Init();

            _longestMatchWasFound = false;
            _optimumEndIndex = 0;
            _optimumCurrentIndex = 0;
            _additionalOffset = 0;
        }

        private void ReadMatchDistances(out uint lenRes, out uint numDistancePairs)
        {
            lenRes = 0;
            numDistancePairs = _matchFinder.GetMatches(_matchDistances);
            if (numDistancePairs > 0)
            {
                lenRes = _matchDistances[numDistancePairs - 2];
                if (lenRes == _numFastBytes)
                    lenRes += _matchFinder.GetMatchLen((int) lenRes - 1, _matchDistances[numDistancePairs - 1],
                        Base.MatchMaxLen - lenRes);
            }

            _additionalOffset++;
        }


        private void MovePos(uint num)
        {
            if (num <= 0) return;

            _matchFinder.Skip(num);
            _additionalOffset += num;
        }

        private uint GetRepLen1Price(Base.State state, uint posState) =>
            _isRepG0[state.Index].GetPrice0() +
            _isRep0Long[(state.Index << Base.NumPosStatesBitsMax) + posState].GetPrice0();

        private uint GetPureRepPrice(uint repIndex, Base.State state, uint posState)
        {
            uint price;
            if (repIndex == 0)
            {
                price = _isRepG0[state.Index].GetPrice0();
                price += _isRep0Long[(state.Index << Base.NumPosStatesBitsMax) + posState].GetPrice1();
            }
            else
            {
                price = _isRepG0[state.Index].GetPrice1();
                if (repIndex == 1)
                    price += _isRepG1[state.Index].GetPrice0();
                else
                {
                    price += _isRepG1[state.Index].GetPrice1();
                    price += _isRepG2[state.Index].GetPrice(repIndex - 2);
                }
            }

            return price;
        }

        private uint GetRepPrice(uint repIndex, uint len, Base.State state, uint posState)
        {
            var price = _repMatchLenEncoder.GetPrice(len - Base.MatchMinLen, posState);
            return price + GetPureRepPrice(repIndex, state, posState);
        }

        private uint GetPosLenPrice(uint pos, uint len, uint posState)
        {
            uint price;
            var lenToPosState = Base.GetLenToPosState(len);
            if (pos < Base.NumFullDistances)
                price = _distancesPrices[lenToPosState * Base.NumFullDistances + pos];
            else
                price = _posSlotPrices[(lenToPosState << Base.NumPosSlotBits) + GetPosSlot2(pos)] +
                        _alignPrices[pos & Base.AlignMask];
            return price + _lenEncoder.GetPrice(len - Base.MatchMinLen, posState);
        }

        private uint Backward(out uint backRes, uint cur)
        {
            _optimumEndIndex = cur;
            var posMem = _optimum[cur].PosPrev;
            var backMem = _optimum[cur].BackPrev;
            do
            {
                if (_optimum[cur].Prev1IsChar)
                {
                    _optimum[posMem].MakeAsChar();
                    _optimum[posMem].PosPrev = posMem - 1;
                    if (_optimum[cur].Prev2)
                    {
                        _optimum[posMem - 1].Prev1IsChar = false;
                        _optimum[posMem - 1].PosPrev = _optimum[cur].PosPrev2;
                        _optimum[posMem - 1].BackPrev = _optimum[cur].BackPrev2;
                    }
                }

                var posPrev = posMem;
                var backCur = backMem;

                backMem = _optimum[posPrev].BackPrev;
                posMem = _optimum[posPrev].PosPrev;

                _optimum[posPrev].BackPrev = backCur;
                _optimum[posPrev].PosPrev = cur;
                cur = posPrev;
            } while (cur > 0);

            backRes = _optimum[0].BackPrev;
            _optimumCurrentIndex = _optimum[0].PosPrev;
            return _optimumCurrentIndex;
        }


        private uint GetOptimum(uint position, out uint backRes)
        {
            if (_optimumEndIndex != _optimumCurrentIndex)
            {
                var lenRes = _optimum[_optimumCurrentIndex].PosPrev - _optimumCurrentIndex;
                backRes = _optimum[_optimumCurrentIndex].BackPrev;
                _optimumCurrentIndex = _optimum[_optimumCurrentIndex].PosPrev;
                return lenRes;
            }

            _optimumCurrentIndex = _optimumEndIndex = 0;

            uint lenMain, numDistancePairs;
            if (!_longestMatchWasFound)
                ReadMatchDistances(out lenMain, out numDistancePairs);
            else
            {
                lenMain = _longestMatchLength;
                numDistancePairs = _numDistancePairs;
                _longestMatchWasFound = false;
            }

            var numAvailableBytes = _matchFinder.GetNumAvailableBytes() + 1;
            if (numAvailableBytes < 2)
            {
                backRes = 0xFFFFFFFF;
                return 1;
            }
            
            uint repMaxIndex = 0;
            uint i;
            for (i = 0; i < Base.NumRepDistances; i++)
            {
                _reps[i] = _repDistances[i];
                _repLens[i] = _matchFinder.GetMatchLen(0 - 1, _reps[i], Base.MatchMaxLen);
                if (_repLens[i] > _repLens[repMaxIndex])
                    repMaxIndex = i;
            }

            if (_repLens[repMaxIndex] >= _numFastBytes)
            {
                backRes = repMaxIndex;
                var lenRes = _repLens[repMaxIndex];
                MovePos(lenRes - 1);
                return lenRes;
            }

            if (lenMain >= _numFastBytes)
            {
                backRes = _matchDistances[numDistancePairs - 1] + Base.NumRepDistances;
                MovePos(lenMain - 1);
                return lenMain;
            }

            var currentByte = _matchFinder.GetIndexByte(0 - 1);
            var matchByte = _matchFinder.GetIndexByte((int) (0 - _repDistances[0] - 1 - 1));

            if (lenMain < 2 && currentByte != matchByte && _repLens[repMaxIndex] < 2)
            {
                backRes = 0xFFFFFFFF;
                return 1;
            }

            _optimum[0].State = _state;

            var posState = position & _posStateMask;

            _optimum[1].Price = _isMatch[(_state.Index << Base.NumPosStatesBitsMax) + posState].GetPrice0() +
                                _literalEncoder.GetSubCoder(position, _previousByte).GetPrice(!_state.IsCharState(), matchByte, currentByte);
            _optimum[1].MakeAsChar();

            var matchPrice = _isMatch[(_state.Index << Base.NumPosStatesBitsMax) + posState].GetPrice1();
            var repMatchPrice = matchPrice + _isRep[_state.Index].GetPrice1();

            if (matchByte == currentByte)
            {
                var shortRepPrice = repMatchPrice + GetRepLen1Price(_state, posState);
                if (shortRepPrice < _optimum[1].Price)
                {
                    _optimum[1].Price = shortRepPrice;
                    _optimum[1].MakeAsShortRep();
                }
            }

            var lenEnd = lenMain >= _repLens[repMaxIndex] ? lenMain : _repLens[repMaxIndex];

            if (lenEnd < 2)
            {
                backRes = _optimum[1].BackPrev;
                return 1;
            }

            _optimum[1].PosPrev = 0;

            _optimum[0].Backs0 = _reps[0];
            _optimum[0].Backs1 = _reps[1];
            _optimum[0].Backs2 = _reps[2];
            _optimum[0].Backs3 = _reps[3];

            var len = lenEnd;
            do
            {
                _optimum[len--].Price = kIfinityPrice;
            } while (len >= 2);

            for (i = 0; i < Base.NumRepDistances; i++)
            {
                var repLen = _repLens[i];
                if (repLen < 2)
                    continue;
                var price = repMatchPrice + GetPureRepPrice(i, _state, posState);
                do
                {
                    var curAndLenPrice = price + _repMatchLenEncoder.GetPrice(repLen - 2, posState);
                    var optimum = _optimum[repLen];
                    if (curAndLenPrice >= optimum.Price) continue;

                    optimum.Price = curAndLenPrice;
                    optimum.PosPrev = 0;
                    optimum.BackPrev = i;
                    optimum.Prev1IsChar = false;
                } while (--repLen >= 2);
            }

            var normalMatchPrice = matchPrice + _isRep[_state.Index].GetPrice0();

            len = _repLens[0] >= 2 ? _repLens[0] + 1 : 2;
            if (len <= lenMain)
            {
                uint offs = 0;
                while (len > _matchDistances[offs])
                    offs += 2;
                for (;; len++)
                {
                    var distance = _matchDistances[offs + 1];
                    var curAndLenPrice = normalMatchPrice + GetPosLenPrice(distance, len, posState);
                    var optimum = _optimum[len];
                    if (curAndLenPrice < optimum.Price)
                    {
                        optimum.Price = curAndLenPrice;
                        optimum.PosPrev = 0;
                        optimum.BackPrev = distance + Base.NumRepDistances;
                        optimum.Prev1IsChar = false;
                    }

                    if (len != _matchDistances[offs]) continue;

                    offs += 2;
                    if (offs == numDistancePairs)
                        break;
                }
            }

            uint cur = 0;

            while (true)
            {
                cur++;
                if (cur == lenEnd)
                    return Backward(out backRes, cur);
                ReadMatchDistances(out var newLen, out numDistancePairs);
                if (newLen >= _numFastBytes)
                {
                    _numDistancePairs = numDistancePairs;
                    _longestMatchLength = newLen;
                    _longestMatchWasFound = true;
                    return Backward(out backRes, cur);
                }

                position++;
                var posPrev = _optimum[cur].PosPrev;
                Base.State state;
                if (_optimum[cur].Prev1IsChar)
                {
                    posPrev--;
                    if (_optimum[cur].Prev2)
                    {
                        state = _optimum[_optimum[cur].PosPrev2].State;
                        if (_optimum[cur].BackPrev2 < Base.NumRepDistances)
                            state.UpdateRep();
                        else
                            state.UpdateMatch();
                    }
                    else
                        state = _optimum[posPrev].State;

                    state.UpdateChar();
                }
                else
                    state = _optimum[posPrev].State;

                if (posPrev == cur - 1)
                {
                    if (_optimum[cur].IsShortRep())
                        state.UpdateShortRep();
                    else
                        state.UpdateChar();
                }
                else
                {
                    uint pos;
                    if (_optimum[cur].Prev1IsChar && _optimum[cur].Prev2)
                    {
                        posPrev = _optimum[cur].PosPrev2;
                        pos = _optimum[cur].BackPrev2;
                        state.UpdateRep();
                    }
                    else
                    {
                        pos = _optimum[cur].BackPrev;
                        if (pos < Base.NumRepDistances)
                            state.UpdateRep();
                        else
                            state.UpdateMatch();
                    }

                    var opt = _optimum[posPrev];
                    if (pos < Base.NumRepDistances)
                    {
                        switch (pos)
                        {
                            case 0:
                                _reps[0] = opt.Backs0;
                                _reps[1] = opt.Backs1;
                                _reps[2] = opt.Backs2;
                                _reps[3] = opt.Backs3;
                                break;
                            case 1:
                                _reps[0] = opt.Backs1;
                                _reps[1] = opt.Backs0;
                                _reps[2] = opt.Backs2;
                                _reps[3] = opt.Backs3;
                                break;
                            case 2:
                                _reps[0] = opt.Backs2;
                                _reps[1] = opt.Backs0;
                                _reps[2] = opt.Backs1;
                                _reps[3] = opt.Backs3;
                                break;
                            default:
                                _reps[0] = opt.Backs3;
                                _reps[1] = opt.Backs0;
                                _reps[2] = opt.Backs1;
                                _reps[3] = opt.Backs2;
                                break;
                        }
                    }
                    else
                    {
                        _reps[0] = pos - Base.NumRepDistances;
                        _reps[1] = opt.Backs0;
                        _reps[2] = opt.Backs1;
                        _reps[3] = opt.Backs2;
                    }
                }

                _optimum[cur].State = state;
                _optimum[cur].Backs0 = _reps[0];
                _optimum[cur].Backs1 = _reps[1];
                _optimum[cur].Backs2 = _reps[2];
                _optimum[cur].Backs3 = _reps[3];
                var curPrice = _optimum[cur].Price;

                currentByte = _matchFinder.GetIndexByte(0 - 1);
                matchByte = _matchFinder.GetIndexByte((int) (0 - _reps[0] - 1 - 1));

                posState = position & _posStateMask;

                var curAnd1Price = curPrice +
                                   _isMatch[(state.Index << Base.NumPosStatesBitsMax) + posState].GetPrice0() +
                                   _literalEncoder.GetSubCoder(position, _matchFinder.GetIndexByte(0 - 2)).GetPrice(!state.IsCharState(), matchByte, currentByte);

                var nextOptimum = _optimum[cur + 1];

                var nextIsChar = false;
                if (curAnd1Price < nextOptimum.Price)
                {
                    nextOptimum.Price = curAnd1Price;
                    nextOptimum.PosPrev = cur;
                    nextOptimum.MakeAsChar();
                    nextIsChar = true;
                }

                matchPrice = curPrice + _isMatch[(state.Index << Base.NumPosStatesBitsMax) + posState].GetPrice1();
                repMatchPrice = matchPrice + _isRep[state.Index].GetPrice1();

                if (matchByte == currentByte &&
                    !(nextOptimum.PosPrev < cur && nextOptimum.BackPrev == 0))
                {
                    var shortRepPrice = repMatchPrice + GetRepLen1Price(state, posState);
                    if (shortRepPrice <= nextOptimum.Price)
                    {
                        nextOptimum.Price = shortRepPrice;
                        nextOptimum.PosPrev = cur;
                        nextOptimum.MakeAsShortRep();
                        nextIsChar = true;
                    }
                }

                var numAvailableBytesFull = _matchFinder.GetNumAvailableBytes() + 1;
                numAvailableBytesFull = Math.Min(kNumOpts - 1 - cur, numAvailableBytesFull);
                numAvailableBytes = numAvailableBytesFull;

                if (numAvailableBytes < 2)
                    continue;
                if (numAvailableBytes > _numFastBytes)
                    numAvailableBytes = _numFastBytes;
                if (!nextIsChar && matchByte != currentByte)
                {
                    // try Literal + rep0
                    var t = Math.Min(numAvailableBytesFull - 1, _numFastBytes);
                    var lenTest2 = _matchFinder.GetMatchLen(0, _reps[0], t);
                    if (lenTest2 >= 2)
                    {
                        var state2 = state;
                        state2.UpdateChar();
                        var posStateNext = (position + 1) & _posStateMask;
                        var nextRepMatchPrice = curAnd1Price +
                                                _isMatch[(state2.Index << Base.NumPosStatesBitsMax) + posStateNext].GetPrice1() +
                                                _isRep[state2.Index].GetPrice1();
                        {
                            var offset = cur + 1 + lenTest2;
                            while (lenEnd < offset)
                                _optimum[++lenEnd].Price = kIfinityPrice;
                            var curAndLenPrice = nextRepMatchPrice + GetRepPrice(
                                                     0, lenTest2, state2, posStateNext);
                            var optimum = _optimum[offset];
                            if (curAndLenPrice < optimum.Price)
                            {
                                optimum.Price = curAndLenPrice;
                                optimum.PosPrev = cur + 1;
                                optimum.BackPrev = 0;
                                optimum.Prev1IsChar = true;
                                optimum.Prev2 = false;
                            }
                        }
                    }
                }

                uint startLen = 2; // speed optimization 

                for (uint repIndex = 0; repIndex < Base.NumRepDistances; repIndex++)
                {
                    var lenTest = _matchFinder.GetMatchLen(0 - 1, _reps[repIndex], numAvailableBytes);
                    if (lenTest < 2)
                        continue;
                    var lenTestTemp = lenTest;
                    do
                    {
                        while (lenEnd < cur + lenTest)
                            _optimum[++lenEnd].Price = kIfinityPrice;
                        var curAndLenPrice = repMatchPrice + GetRepPrice(repIndex, lenTest, state, posState);
                        var optimum = _optimum[cur + lenTest];
                        if (curAndLenPrice < optimum.Price)
                        {
                            optimum.Price = curAndLenPrice;
                            optimum.PosPrev = cur;
                            optimum.BackPrev = repIndex;
                            optimum.Prev1IsChar = false;
                        }
                    } while (--lenTest >= 2);

                    lenTest = lenTestTemp;

                    if (repIndex == 0)
                        startLen = lenTest + 1;

                    // if (_maxMode)
                    if (lenTest >= numAvailableBytesFull) continue;

                    var t = Math.Min(numAvailableBytesFull - 1 - lenTest, _numFastBytes);
                    var lenTest2 = _matchFinder.GetMatchLen((int) lenTest, _reps[repIndex], t);
                    if (lenTest2 < 2) continue;

                    var state2 = state;
                    state2.UpdateRep();
                    var posStateNext = (position + lenTest) & _posStateMask;
                    var curAndLenCharPrice =
                        repMatchPrice + GetRepPrice(repIndex, lenTest, state, posState) +
                        _isMatch[(state2.Index << Base.NumPosStatesBitsMax) + posStateNext].GetPrice0() +
                        _literalEncoder.GetSubCoder(position + lenTest,
                            _matchFinder.GetIndexByte((int) lenTest - 1 - 1)).GetPrice(true,
                            _matchFinder.GetIndexByte((int) lenTest - 1 - (int) (_reps[repIndex] + 1)),
                            _matchFinder.GetIndexByte((int) lenTest - 1));
                    state2.UpdateChar();
                    posStateNext = (position + lenTest + 1) & _posStateMask;
                    var nextMatchPrice = curAndLenCharPrice + _isMatch[(state2.Index << Base.NumPosStatesBitsMax) + posStateNext].GetPrice1();
                    var nextRepMatchPrice = nextMatchPrice + _isRep[state2.Index].GetPrice1();

                    // for(; lenTest2 >= 2; lenTest2--)
                    {
                        var offset = lenTest + 1 + lenTest2;
                        while (lenEnd < cur + offset)
                            _optimum[++lenEnd].Price = kIfinityPrice;
                        var curAndLenPrice = nextRepMatchPrice + GetRepPrice(0, lenTest2, state2, posStateNext);
                        var optimum = _optimum[cur + offset];
                        if (curAndLenPrice < optimum.Price)
                        {
                            optimum.Price = curAndLenPrice;
                            optimum.PosPrev = cur + lenTest + 1;
                            optimum.BackPrev = 0;
                            optimum.Prev1IsChar = true;
                            optimum.Prev2 = true;
                            optimum.PosPrev2 = cur;
                            optimum.BackPrev2 = repIndex;
                        }
                    }
                }

                if (newLen > numAvailableBytes)
                {
                    newLen = numAvailableBytes;
                    for (numDistancePairs = 0; newLen > _matchDistances[numDistancePairs]; numDistancePairs += 2)
                    {}

                    _matchDistances[numDistancePairs] = newLen;
                    numDistancePairs += 2;
                }

                if (newLen < startLen) continue;

                normalMatchPrice = matchPrice + _isRep[state.Index].GetPrice0();
                while (lenEnd < cur + newLen)
                    _optimum[++lenEnd].Price = kIfinityPrice;

                uint offs = 0;
                while (startLen > _matchDistances[offs])
                    offs += 2;

                for (var lenTest = startLen;; lenTest++)
                {
                    var curBack = _matchDistances[offs + 1];
                    var curAndLenPrice = normalMatchPrice + GetPosLenPrice(curBack, lenTest, posState);
                    var optimum = _optimum[cur + lenTest];
                    if (curAndLenPrice < optimum.Price)
                    {
                        optimum.Price = curAndLenPrice;
                        optimum.PosPrev = cur;
                        optimum.BackPrev = curBack + Base.NumRepDistances;
                        optimum.Prev1IsChar = false;
                    }

                    if (lenTest != _matchDistances[offs]) continue;

                    if (lenTest < numAvailableBytesFull)
                    {
                        var t = Math.Min(numAvailableBytesFull - 1 - lenTest, _numFastBytes);
                        var lenTest2 = _matchFinder.GetMatchLen((int) lenTest, curBack, t);
                        if (lenTest2 >= 2)
                        {
                            var state2 = state;
                            state2.UpdateMatch();
                            var posStateNext = (position + lenTest) & _posStateMask;
                            var curAndLenCharPrice = curAndLenPrice +
                                                     _isMatch[(state2.Index << Base.NumPosStatesBitsMax) + posStateNext].GetPrice0() +
                                                     _literalEncoder.GetSubCoder(position + lenTest,
                                                         _matchFinder.GetIndexByte((int) lenTest - 1 - 1)).GetPrice(true,
                                                         _matchFinder.GetIndexByte((int) lenTest - (int) (curBack + 1) - 1),
                                                         _matchFinder.GetIndexByte((int) lenTest - 1));
                            state2.UpdateChar();
                            posStateNext = (position + lenTest + 1) & _posStateMask;
                            var nextMatchPrice = curAndLenCharPrice + _isMatch[(state2.Index << Base.NumPosStatesBitsMax) + posStateNext].GetPrice1();
                            var nextRepMatchPrice = nextMatchPrice + _isRep[state2.Index].GetPrice1();

                            var offset = lenTest + 1 + lenTest2;
                            while (lenEnd < cur + offset)
                                _optimum[++lenEnd].Price = kIfinityPrice;
                            curAndLenPrice = nextRepMatchPrice + GetRepPrice(0, lenTest2, state2, posStateNext);
                            optimum = _optimum[cur + offset];
                            if (curAndLenPrice < optimum.Price)
                            {
                                optimum.Price = curAndLenPrice;
                                optimum.PosPrev = cur + lenTest + 1;
                                optimum.BackPrev = 0;
                                optimum.Prev1IsChar = true;
                                optimum.Prev2 = true;
                                optimum.PosPrev2 = cur;
                                optimum.BackPrev2 = curBack + Base.NumRepDistances;
                            }
                        }
                    }

                    offs += 2;
                    if (offs == numDistancePairs)
                        break;
                }
            }
        }

        private void WriteEndMarker(uint posState)
        {
            if (!_writeEndMark)
                return;

            _isMatch[(_state.Index << Base.NumPosStatesBitsMax) + posState].Encode(_rangeEncoder, 1);
            _isRep[_state.Index].Encode(_rangeEncoder, 0);
            _state.UpdateMatch();
            var len = Base.MatchMinLen;
            _lenEncoder.Encode(_rangeEncoder, len - Base.MatchMinLen, posState);
            uint posSlot = (1 << Base.NumPosSlotBits) - 1;
            var lenToPosState = Base.GetLenToPosState(len);
            _posSlotEncoder[lenToPosState].Encode(_rangeEncoder, posSlot);
            var footerBits = 30;
            var posReduced = ((uint) 1 << footerBits) - 1;
            _rangeEncoder.EncodeDirectBits(posReduced >> Base.NumAlignBits, footerBits - Base.NumAlignBits);
            _posAlignEncoder.ReverseEncode(_rangeEncoder, posReduced & Base.AlignMask);
        }

        private void Flush(uint nowPos)
        {
            ReleaseMfStream();
            WriteEndMarker(nowPos & _posStateMask);
            _rangeEncoder.FlushData();
            _rangeEncoder.FlushStream();
        }

        private void CodeOneBlock(out long inSize, out long outSize, out bool finished)
        {
            inSize = 0;
            outSize = 0;
            finished = true;

            if (_inStream != null)
            {
                _matchFinder.SetStream(_inStream);
                _matchFinder.Init();
                _needReleaseMfStream = true;
                _inStream = null;
                if (_trainSize > 0)
                    _matchFinder.Skip(_trainSize);
            }

            if (_finished)
                return;
            _finished = true;


            var progressPosValuePrev = _nowPos64;
            if (_nowPos64 == 0)
            {
                if (_matchFinder.GetNumAvailableBytes() == 0)
                {
                    Flush((uint) _nowPos64);
                    return;
                }

                ReadMatchDistances(out _, out _);
                var posState = (uint) _nowPos64 & _posStateMask;
                _isMatch[(_state.Index << Base.NumPosStatesBitsMax) + posState].Encode(_rangeEncoder, 0);
                _state.UpdateChar();
                var curByte = _matchFinder.GetIndexByte((int) (0 - _additionalOffset));
                _literalEncoder.GetSubCoder((uint) _nowPos64, _previousByte).Encode(_rangeEncoder, curByte);
                _previousByte = curByte;
                _additionalOffset--;
                _nowPos64++;
            }

            if (_matchFinder.GetNumAvailableBytes() == 0)
            {
                Flush((uint) _nowPos64);
                return;
            }

            while (true)
            {
                var len = GetOptimum((uint) _nowPos64, out var pos);

                var posState = (uint) _nowPos64 & _posStateMask;
                var complexState = (_state.Index << Base.NumPosStatesBitsMax) + posState;
                if (len == 1 && pos == 0xFFFFFFFF)
                {
                    _isMatch[complexState].Encode(_rangeEncoder, 0);
                    var curByte = _matchFinder.GetIndexByte((int) (0 - _additionalOffset));
                    var subCoder = _literalEncoder.GetSubCoder((uint) _nowPos64, _previousByte);
                    if (!_state.IsCharState())
                    {
                        var matchByte = _matchFinder.GetIndexByte((int) (0 - _repDistances[0] - 1 - _additionalOffset));
                        subCoder.EncodeMatched(_rangeEncoder, matchByte, curByte);
                    }
                    else
                        subCoder.Encode(_rangeEncoder, curByte);

                    _previousByte = curByte;
                    _state.UpdateChar();
                }
                else
                {
                    _isMatch[complexState].Encode(_rangeEncoder, 1);
                    if (pos < Base.NumRepDistances)
                    {
                        _isRep[_state.Index].Encode(_rangeEncoder, 1);
                        if (pos == 0)
                        {
                            _isRepG0[_state.Index].Encode(_rangeEncoder, 0);
                            _isRep0Long[complexState].Encode(_rangeEncoder, len == 1 ? 0 : (uint) 1);
                        }
                        else
                        {
                            _isRepG0[_state.Index].Encode(_rangeEncoder, 1);
                            if (pos == 1)
                                _isRepG1[_state.Index].Encode(_rangeEncoder, 0);
                            else
                            {
                                _isRepG1[_state.Index].Encode(_rangeEncoder, 1);
                                _isRepG2[_state.Index].Encode(_rangeEncoder, pos - 2);
                            }
                        }

                        if (len == 1)
                            _state.UpdateShortRep();
                        else
                        {
                            _repMatchLenEncoder.Encode(_rangeEncoder, len - Base.MatchMinLen, posState);
                            _state.UpdateRep();
                        }

                        var distance = _repDistances[pos];
                        if (pos != 0)
                        {
                            for (var i = pos; i >= 1; i--)
                                _repDistances[i] = _repDistances[i - 1];
                            _repDistances[0] = distance;
                        }
                    }
                    else
                    {
                        _isRep[_state.Index].Encode(_rangeEncoder, 0);
                        _state.UpdateMatch();
                        _lenEncoder.Encode(_rangeEncoder, len - Base.MatchMinLen, posState);
                        pos -= Base.NumRepDistances;
                        var posSlot = GetPosSlot(pos);
                        var lenToPosState = Base.GetLenToPosState(len);
                        _posSlotEncoder[lenToPosState].Encode(_rangeEncoder, posSlot);

                        if (posSlot >= Base.StartPosModelIndex)
                        {
                            var footerBits = (int) ((posSlot >> 1) - 1);
                            var baseVal = (2 | (posSlot & 1)) << footerBits;
                            var posReduced = pos - baseVal;

                            if (posSlot < Base.EndPosModelIndex)
                            {
                                BitTreeEncoder.ReverseEncode(_posEncoders,
                                    baseVal - posSlot - 1, _rangeEncoder, footerBits, posReduced);
                            }
                            else
                            {
                                _rangeEncoder.EncodeDirectBits(posReduced >> Base.NumAlignBits, footerBits - Base.NumAlignBits);
                                _posAlignEncoder.ReverseEncode(_rangeEncoder, posReduced & Base.AlignMask);
                                _alignPriceCount++;
                            }
                        }

                        var distance = pos;
                        for (var i = Base.NumRepDistances - 1; i >= 1; i--)
                            _repDistances[i] = _repDistances[i - 1];
                        _repDistances[0] = distance;
                        _matchPriceCount++;
                    }

                    _previousByte = _matchFinder.GetIndexByte((int) (len - 1 - _additionalOffset));
                }

                _additionalOffset -= len;
                _nowPos64 += len;
                if (_additionalOffset == 0)
                {
                    // if (!_fastMode)
                    if (_matchPriceCount >= 1 << 7)
                        FillDistancesPrices();
                    if (_alignPriceCount >= Base.AlignTableSize)
                        FillAlignPrices();
                    inSize = _nowPos64;
                    outSize = _rangeEncoder.GetProcessedSizeAdd();
                    if (_matchFinder.GetNumAvailableBytes() == 0)
                    {
                        Flush((uint) _nowPos64);
                        return;
                    }

                    if (_nowPos64 - progressPosValuePrev >= 1 << 12)
                    {
                        _finished = false;
                        finished = false;
                        return;
                    }
                }
            }
        }

        private void ReleaseMfStream()
        {
            if (_matchFinder == null || !_needReleaseMfStream) return;

            _matchFinder.ReleaseStream();
            _needReleaseMfStream = false;
        }

        private void SetOutStream(Stream outStream) 
            => _rangeEncoder.SetStream(outStream);

        private void ReleaseOutStream() 
            => _rangeEncoder.ReleaseStream();

        private void ReleaseStreams()
        {
            ReleaseMfStream();
            ReleaseOutStream();
        }

        private void SetStreams(Stream inStream, Stream outStream)
        {
            _inStream = inStream;
            _finished = false;
            Create();
            SetOutStream(outStream);
            Init();

            // if (!_fastMode)
            {
                FillDistancesPrices();
                FillAlignPrices();
            }

            _lenEncoder.SetTableSize(_numFastBytes + 1 - Base.MatchMinLen);
            _lenEncoder.UpdateTables((uint) 1 << _posStateBits);
            _repMatchLenEncoder.SetTableSize(_numFastBytes + 1 - Base.MatchMinLen);
            _repMatchLenEncoder.UpdateTables((uint) 1 << _posStateBits);

            _nowPos64 = 0;
        }

        private void FillDistancesPrices()
        {
            for (var i = Base.StartPosModelIndex; i < Base.NumFullDistances; i++)
            {
                var posSlot = GetPosSlot(i);
                var footerBits = (int) ((posSlot >> 1) - 1);
                var baseVal = (2 | (posSlot & 1)) << footerBits;
                _tempPrices[i] = BitTreeEncoder.ReverseGetPrice(_posEncoders,
                    baseVal - posSlot - 1, footerBits, i - baseVal);
            }

            for (uint lenToPosState = 0; lenToPosState < Base.NumLenToPosStates; lenToPosState++)
            {
                uint posSlot;
                var encoder = _posSlotEncoder[lenToPosState];

                var st = lenToPosState << Base.NumPosSlotBits;
                for (posSlot = 0; posSlot < _distTableSize; posSlot++)
                    _posSlotPrices[st + posSlot] = encoder.GetPrice(posSlot);
                for (posSlot = Base.EndPosModelIndex; posSlot < _distTableSize; posSlot++)
                    _posSlotPrices[st + posSlot] += ((posSlot >> 1) - 1 - Base.NumAlignBits) << BitEncoder.kNumBitPriceShiftBits;

                var st2 = lenToPosState * Base.NumFullDistances;
                uint i;
                for (i = 0; i < Base.StartPosModelIndex; i++)
                    _distancesPrices[st2 + i] = _posSlotPrices[st + i];
                for (; i < Base.NumFullDistances; i++)
                    _distancesPrices[st2 + i] = _posSlotPrices[st + GetPosSlot(i)] + _tempPrices[i];
            }

            _matchPriceCount = 0;
        }

        private void FillAlignPrices()
        {
            for (uint i = 0; i < Base.AlignTableSize; i++)
                _alignPrices[i] = _posAlignEncoder.ReverseGetPrice(i);
            _alignPriceCount = 0;
        }

        private static int FindMatchFinder(string s)
        {
            for (var m = 0; m < MatchFinderIDs.Length; m++)
                if (s == MatchFinderIDs[m])
                    return m;
            return -1;
        }

        private enum EMatchFinderType
        {
            Bt2,
            Bt4
        }

        private class LiteralEncoder
        {
            private Encoder2[] _mCoders;
            private int _mNumPosBits;
            private int _mNumPrevBits;
            private uint _mPosMask;

            public void Create(int numPosBits, int numPrevBits)
            {
                if (_mCoders != null && _mNumPrevBits == numPrevBits && _mNumPosBits == numPosBits)
                    return;
                _mNumPosBits = numPosBits;
                _mPosMask = ((uint) 1 << numPosBits) - 1;
                _mNumPrevBits = numPrevBits;
                var numStates = (uint) 1 << (_mNumPrevBits + _mNumPosBits);
                _mCoders = new Encoder2[numStates];
                for (uint i = 0; i < numStates; i++)
                    _mCoders[i].Create();
            }

            public void Init()
            {
                var numStates = (uint) 1 << (_mNumPrevBits + _mNumPosBits);
                for (uint i = 0; i < numStates; i++)
                    _mCoders[i].Init();
            }

            public Encoder2 GetSubCoder(uint pos, byte prevByte) 
                => _mCoders[((pos & _mPosMask) << _mNumPrevBits) + (uint) (prevByte >> (8 - _mNumPrevBits))];

            public struct Encoder2
            {
                private BitEncoder[] _mEncoders;

                public void Create() 
                    => _mEncoders = new BitEncoder[0x300];

                public void Init()
                {
                    for (var i = 0; i < 0x300; i++) _mEncoders[i].Init();
                }

                public void Encode(RangeCoder.Encoder rangeEncoder, byte symbol)
                {
                    uint context = 1;
                    for (var i = 7; i >= 0; i--)
                    {
                        var bit = (uint) ((symbol >> i) & 1);
                        _mEncoders[context].Encode(rangeEncoder, bit);
                        context = (context << 1) | bit;
                    }
                }

                public void EncodeMatched(RangeCoder.Encoder rangeEncoder, byte matchByte, byte symbol)
                {
                    uint context = 1;
                    var same = true;
                    for (var i = 7; i >= 0; i--)
                    {
                        var bit = (uint) ((symbol >> i) & 1);
                        var state = context;
                        if (same)
                        {
                            var matchBit = (uint) ((matchByte >> i) & 1);
                            state += (1 + matchBit) << 8;
                            same = matchBit == bit;
                        }

                        _mEncoders[state].Encode(rangeEncoder, bit);
                        context = (context << 1) | bit;
                    }
                }

                public uint GetPrice(bool matchMode, byte matchByte, byte symbol)
                {
                    uint price = 0;
                    uint context = 1;
                    var i = 7;
                    if (matchMode)
                        for (; i >= 0; i--)
                        {
                            var matchBit = (uint) (matchByte >> i) & 1;
                            var bit = (uint) (symbol >> i) & 1;
                            price += _mEncoders[((1 + matchBit) << 8) + context].GetPrice(bit);
                            context = (context << 1) | bit;
                            if (matchBit != bit)
                            {
                                i--;
                                break;
                            }
                        }

                    for (; i >= 0; i--)
                    {
                        var bit = (uint) (symbol >> i) & 1;
                        price += _mEncoders[context].GetPrice(bit);
                        context = (context << 1) | bit;
                    }

                    return price;
                }
            }
        }

        private class LenEncoder
        {
            private BitEncoder _choice = new BitEncoder();
            private BitEncoder _choice2 = new BitEncoder();
            private BitTreeEncoder _highCoder = new BitTreeEncoder(Base.NumHighLenBits);
            private readonly BitTreeEncoder[] _lowCoder = new BitTreeEncoder[Base.NumPosStatesEncodingMax];
            private readonly BitTreeEncoder[] _midCoder = new BitTreeEncoder[Base.NumPosStatesEncodingMax];

            protected LenEncoder()
            {
                for (uint posState = 0; posState < Base.NumPosStatesEncodingMax; posState++)
                {
                    _lowCoder[posState] = new BitTreeEncoder(Base.NumLowLenBits);
                    _midCoder[posState] = new BitTreeEncoder(Base.NumMidLenBits);
                }
            }

            public void Init(uint numPosStates)
            {
                _choice.Init();
                _choice2.Init();
                for (uint posState = 0; posState < numPosStates; posState++)
                {
                    _lowCoder[posState].Init();
                    _midCoder[posState].Init();
                }

                _highCoder.Init();
            }

            protected void Encode(RangeCoder.Encoder rangeEncoder, uint symbol, uint posState)
            {
                if (symbol < Base.NumLowLenSymbols)
                {
                    _choice.Encode(rangeEncoder, 0);
                    _lowCoder[posState].Encode(rangeEncoder, symbol);
                }
                else
                {
                    symbol -= Base.NumLowLenSymbols;
                    _choice.Encode(rangeEncoder, 1);
                    if (symbol < Base.NumMidLenSymbols)
                    {
                        _choice2.Encode(rangeEncoder, 0);
                        _midCoder[posState].Encode(rangeEncoder, symbol);
                    }
                    else
                    {
                        _choice2.Encode(rangeEncoder, 1);
                        _highCoder.Encode(rangeEncoder, symbol - Base.NumMidLenSymbols);
                    }
                }
            }

            protected void SetPrices(uint posState, uint numSymbols, uint[] prices, uint st)
            {
                var a0 = _choice.GetPrice0();
                var a1 = _choice.GetPrice1();
                var b0 = a1 + _choice2.GetPrice0();
                var b1 = a1 + _choice2.GetPrice1();
                uint i;
                for (i = 0; i < Base.NumLowLenSymbols; i++)
                {
                    if (i >= numSymbols)
                        return;
                    prices[st + i] = a0 + _lowCoder[posState].GetPrice(i);
                }

                for (; i < Base.NumLowLenSymbols + Base.NumMidLenSymbols; i++)
                {
                    if (i >= numSymbols)
                        return;
                    prices[st + i] = b0 + _midCoder[posState].GetPrice(i - Base.NumLowLenSymbols);
                }

                for (; i < numSymbols; i++)
                    prices[st + i] = b1 + _highCoder.GetPrice(i - Base.NumLowLenSymbols - Base.NumMidLenSymbols);
            }
        }

        private class LenPriceTableEncoder : LenEncoder
        {
            private readonly uint[] _counters = new uint[Base.NumPosStatesEncodingMax];
            private readonly uint[] _prices = new uint[Base.NumLenSymbols << Base.NumPosStatesBitsEncodingMax];
            private uint _tableSize;

            public void SetTableSize(uint tableSize) 
                => _tableSize = tableSize;

            public uint GetPrice(uint symbol, uint posState) 
                => _prices[posState * Base.NumLenSymbols + symbol];

            private void UpdateTable(uint posState)
            {
                SetPrices(posState, _tableSize, _prices, posState * Base.NumLenSymbols);
                _counters[posState] = _tableSize;
            }

            public void UpdateTables(uint numPosStates)
            {
                for (uint posState = 0; posState < numPosStates; posState++)
                    UpdateTable(posState);
            }

            public new void Encode(RangeCoder.Encoder rangeEncoder, uint symbol, uint posState)
            {
                base.Encode(rangeEncoder, symbol, posState);
                if (--_counters[posState] == 0)
                    UpdateTable(posState);
            }
        }

        private class Optimal
        {
            public uint BackPrev;
            public uint BackPrev2;

            public uint Backs0;
            public uint Backs1;
            public uint Backs2;
            public uint Backs3;
            public uint PosPrev;

            public uint PosPrev2;

            public bool Prev1IsChar;
            public bool Prev2;

            public uint Price;
            public Base.State State;

            public void MakeAsChar()
            {
                BackPrev = 0xFFFFFFFF;
                Prev1IsChar = false;
            }

            public void MakeAsShortRep()
            {
                BackPrev = 0;
                Prev1IsChar = false;
            }

            public bool IsShortRep() 
                => BackPrev == 0;
        }
    }
}