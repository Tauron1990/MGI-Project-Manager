using System.IO.Compression;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TestConsoleApp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var test = new AssemblyLoadContext("MSBuild", true);

            test.Unload();
        }
    }
}