// LzmaBase.cs

namespace Tauron.MgiProjectManager.Data.Compressor._7zip.Compress.LZMA
{
    internal abstract class Base
    {
        public const uint NumRepDistances = 4;
        public const uint NumStates = 12;

        public const int NumPosSlotBits = 6;

        public const int DicLogSizeMin = 0;
        // public const int kDicLogSizeMax = 30;
        // public const uint kDistTableSizeMax = kDicLogSizeMax * 2;

        public const int NumLenToPosStatesBits = 2; // it's for speed optimization
        public const uint NumLenToPosStates = 1 << NumLenToPosStatesBits;

        public const uint MatchMinLen = 2;

        public const int NumAlignBits = 4;
        public const uint AlignTableSize = 1 << NumAlignBits;
        public const uint AlignMask = AlignTableSize - 1;

        public const uint StartPosModelIndex = 4;
        public const uint EndPosModelIndex = 14;

        public const uint NumFullDistances = 1 << ((int) EndPosModelIndex / 2);

        public const uint NumLitPosStatesBitsEncodingMax = 4;
        public const uint NumLitContextBitsMax = 8;

        public const int NumPosStatesBitsMax = 4;
        public const uint NumPosStatesMax = 1 << NumPosStatesBitsMax;
        public const int NumPosStatesBitsEncodingMax = 4;
        public const uint NumPosStatesEncodingMax = 1 << NumPosStatesBitsEncodingMax;

        public const int NumLowLenBits = 3;
        public const int NumMidLenBits = 3;
        public const int NumHighLenBits = 8;
        public const uint NumLowLenSymbols = 1 << NumLowLenBits;
        public const uint NumMidLenSymbols = 1 << NumMidLenBits;

        public const uint NumLenSymbols = NumLowLenSymbols + NumMidLenSymbols +
                                           (1 << NumHighLenBits);

        public const uint MatchMaxLen = MatchMinLen + NumLenSymbols - 1;

        public static uint GetLenToPosState(uint len)
        {
            len -= MatchMinLen;
            if (len < NumLenToPosStates)
                return len;
            return NumLenToPosStates - 1;
        }

        // static byte []kLiteralNextStates  = {0, 0, 0, 0, 1, 2, 3, 4,  5,  6,   4, 5};
        // static byte []kMatchNextStates    = {7, 7, 7, 7, 7, 7, 7, 10, 10, 10, 10, 10};
        // static byte []kRepNextStates      = {8, 8, 8, 8, 8, 8, 8, 11, 11, 11, 11, 11};
        // static byte []kShortRepNextStates = {9, 9, 9, 9, 9, 9, 9, 11, 11, 11, 11, 11};

        public struct State
        {
            public uint Index;

            public void Init() => Index = 0;

            public void UpdateChar()
            {
                if (Index < 4) Index = 0;
                else if (Index < 10) Index -= 3;
                else Index -= 6;
            }

            public void UpdateMatch() 
                => Index = (uint) (Index < 7 ? 7 : 10);

            public void UpdateRep() 
                => Index = (uint) (Index < 7 ? 8 : 11);

            public void UpdateShortRep() 
                => Index = (uint) (Index < 7 ? 9 : 11);

            public bool IsCharState() 
                => Index < 7;
        }
    }
}