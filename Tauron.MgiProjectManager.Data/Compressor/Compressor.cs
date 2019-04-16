using System;
using System.IO;
using System.Threading.Tasks;

namespace Tauron.MgiProjectManager.Data.Compressor
{
    public static class Compressor
    {
        public static async Task CompressFileLzma(Stream input, Stream output)
        {
            SevenZip.Compression.LZMA.Encoder coder = new SevenZip.Compression.LZMA.Encoder();

            // Write the encoder properties
            coder.WriteCoderProperties(output);

            // Write the decompressed file size.
            await output.WriteAsync(BitConverter.GetBytes(input.Length), 0, 8);

            // Encode the file.
            coder.Code(input, output, input.Length, -1, null);
            await output.FlushAsync();
        }

        public static async Task DecompressFileLzma(Stream input, Stream output)
        {
            SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();

            // Read the decoder properties
            byte[] properties = new byte[5];
            await input.ReadAsync(properties, 0, 5);

            // Read in the decompress file size.
            byte[] fileLengthBytes = new byte[8];
            input.Read(fileLengthBytes, 0, 8);
            long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

            coder.SetDecoderProperties(properties);
            coder.Code(input, output, input.Length, fileLength, null);
            await output.FlushAsync();
        }
    }
}