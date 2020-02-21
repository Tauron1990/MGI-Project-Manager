using System.IO.Compression;

namespace TestConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ZipFile.ExtractToDirectory("Test.zip", "Test");
        }
    }
}