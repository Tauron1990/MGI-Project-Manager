using System.IO.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ZipFile.ExtractToDirectory("Test.zip", "Test");
        }
    }
}
