/*
 * Copyright (c) 2000-2006 Joachim Henke
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 *  - Redistributions of source code must retain the above copyright notice,
 *    this list of conditions and the following disclaimer.
 *  - Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 *  - Neither the name of Joachim Henke nor the names of his contributors may
 *    be used to endorse or promote products derived from this software without
 *    specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core
{
    public static class Base91
    {
        private static readonly char[] EncodeTable =
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '!', '#', '$',
            '%', '&', '(', ')', '*', '+', ',', '.', '/', ':', ';', '<', '=',
            '>', '?', '@', '[', ']', '^', '_', '`', '{', '|', '}', '~', '"'
        };

        private static readonly Dictionary<byte, int> DecodeTable = InitDecodeTable();

        [NotNull]
        private static Dictionary<byte, int> InitDecodeTable()
        {
            var decodeTable = new Dictionary<byte, int>();
            for (var i = 0; i < 255; i++) decodeTable[(byte) i] = -1;
            for (var i = 0; i < EncodeTable.Length; i++) decodeTable[(byte) EncodeTable[i]] = i;

            return decodeTable;
        }

        [NotNull]
        public static string Encode([NotNull] IEnumerable<byte> input)
        {
            var output = "";
            var b = 0;
            var n = 0;
            foreach (var t in Argument.NotNull(input, nameof(input)))
            {
                b |= t << n;
                n += 8;
                if (n <= 13) continue;

                var v = b & 8191;
                if (v > 88)
                {
                    b >>= 13;
                    n -= 13;
                }
                else
                {
                    v = b & 16383;
                    b >>= 14;
                    n -= 14;
                }

                output += EncodeTable[v % 91];
                output += EncodeTable[v / 91];
            }

            if (n == 0) return output;

            output += EncodeTable[b % 91];
            if (n > 7 || b > 90) output += EncodeTable[b / 91];
            return output;
        }

        [NotNull]
        public static byte[] Decode([NotNull] string input)
        {
            Argument.NotNull(input, nameof(input));

            var output = new byte[input.Length];
            var v = -1;
            var b = 0;
            var n = 0;
            var d = 0;

            foreach (var c in input.Select(t => DecodeTable[(byte) t]).Where(c => c != -1))
                if (v < 0)
                {
                    v = c;
                }
                else
                {
                    v += c * 91;
                    b |= v << n;
                    n += (v & 8191) > 88 ? 13 : 14;
                    do
                    {
                        output[d++] = (byte) (b & 255);
                        b >>= 8;
                        n -= 8;
                    } while (n > 7);

                    v = -1;
                }

            if (v + 1 != 0) output[d++] = (byte) ((b | (v << n)) & 255);

            var retout = new byte[d];
            Buffer.BlockCopy(output, 0, retout, 0, d);
            return retout;
        }
    }
}