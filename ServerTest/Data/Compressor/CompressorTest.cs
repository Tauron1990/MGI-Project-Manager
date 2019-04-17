using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ServerTest.TestHelper;
using Xunit;
using Xunit.Abstractions;

namespace ServerTest.Data.Compressor
{
    public class CompressorTest : TestBaseClass
    {
        private readonly string _finalString;

        public CompressorTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder builder = new StringBuilder(10485760);
            var random = new Random();

            for (int i = 0; i < 10485760; i++)
            {
                builder.Append(chars[random.Next(chars.Length)]);
            }

            _finalString = builder.ToString();

            TestOutputHelper.WriteLine("Random Text Generatet");
        }

        [Fact(DisplayName = "Test For Stream Compression")]
        public async Task Compress_Decompress_Test()
        {

            var input1 = new MemoryStream(Encoding.UTF8.GetBytes(_finalString));
            var outPut1 = new MemoryStream();

            await Tauron.MgiProjectManager.Data.Compressor.Compressor.CompressFileLzma(input1, outPut1);

            TestOutputHelper.WriteLine($"Compression Compled: {outPut1.Length / 1024d / 1024d} MB");

            input1 = new MemoryStream(outPut1.ToArray());
            outPut1 = new MemoryStream();

            await Tauron.MgiProjectManager.Data.Compressor.Compressor.DecompressFileLzma(input1, outPut1);

            TestOutputHelper.WriteLine($"Decopression Compled:  {outPut1.Length / 1024d / 1024d} MB");

            Assert.Equal(_finalString, Encoding.UTF8.GetString(outPut1.ToArray()));
        } 
    }
}