using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Packaging;
using NuGet.Packaging.Signing;
using RestEase;

namespace TestApp
{
    class ConsoleLogger : LoggerBase
    {
        public override void Log(ILogMessage message)
        {
            Console.WriteLine(message.FormatWithCode());
        }

        public override Task LogAsync(ILogMessage message)
        {
            Console.WriteLine(message.FormatWithCode());

            return Task.CompletedTask;
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            //const string source = @"https://api.nuget.org/v3/index.json";

            //var logger = new ConsoleLogger();
            //var reader = new PackageArchiveReader(@"C:\Users\PC\Desktop\test\MGI\EventDeliveryTest\bin\Release\EventDeliveryTest.1.0.0.nupkg");
            
            Console.WriteLine(RuntimeInformation.FrameworkDescription);

            //await PackageExtractor.ExtractPackageAsync(
            //    "",
            //    reader,
            //    new PackagePathResolver(
            //        Path.Combine(
            //            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            //            "Test")),
            //    new PackageExtractionContext(PackageSaveMode.None, XmlDocFileSaveMode.Skip, ClientPolicyContext.GetClientPolicy(new NullSettings(), new NullLogger()), new NullLogger()),
            //    CancellationToken.None);

            //reader.Dispose();

            Console.ReadKey();
        }
    }
}
