using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading.Tasks;
using CQRSlite.Commands;
using EventDeliveryTest.Test;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tauron.CQRS.Services;
using Tauron.CQRS.Services.Extensions;

namespace EventDeliveryTest
{
    class TestFailed : Exception
    {
        public TestFailed(string msg)
        : base(msg)
        {
            
        }
    }

    class Program
    {
        static async Task Main()
        {
            Console.Title = "Event Delivery Test";

            Console.WriteLine("---Welcome To Event Delivery Test---");
            Console.WriteLine("Please Enter The IP of the Server to Test:");
            Console.Write("IP: ");
            Console.WriteLine("http://localhost:54005");
            Uri ip = new Uri("http://localhost:54005");// Console.ReadLine();
            
            Console.WriteLine();
            Console.WriteLine("Press Enter to Start...");
            Console.ReadKey();
            Console.WriteLine();

            Console.WriteLine("---Tests---");
            try
            {
                Console.WriteLine();

                await PingTest(ip);
                await HealthTest(ip);
                var temp = ServiceCreationTest(ip);
                await TestEventDeleivery(temp);

                Console.WriteLine();
                Console.WriteLine("Tests Completed");
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Tests Failed:");
                Console.WriteLine(e.Demystify());
            }

            Console.ReadKey();
        }

        private static async Task TestEventDeleivery(IServiceProvider serviceProvider)
        {
            Console.Write("Event Delivery Test:");

            using var scope = serviceProvider.CreateScope();
            await scope.ServiceProvider.StartCQRS();

            var awaiter = scope.ServiceProvider.GetRequiredService<AwaiterBase<TestCommand, TestEvent>>();

            const string msg = "Hallo-Welt";

            var (ok, testEvent) = await awaiter.SendAndAwait(new TestCommand { Parameter = msg });

            if (!ok || msg != testEvent.Result) throw new TestFailed($"No Correct repond. Timeout: {!ok}");

            Console.WriteLine(" Success");
        }

        private static IServiceProvider ServiceCreationTest(Uri ip)
        {
            Console.Write("Service Creation Test:");

            IServiceCollection collection = new ServiceCollection();

            collection.AddLogging(lb => lb.AddConsole());
            collection.AddCQRSServices(c => c
                .ScanFrom<Program>()
                .SetUrls(ip, "Temp", "Develop")
                .AddAwaiter<TestCommand, TestEvent>(collection));

            var temp = collection.BuildServiceProvider();
            Console.WriteLine(" Success");

            return temp;
        }

        private static async Task HealthTest(Uri url)
        {
            Console.Write("Health Test:");
            using HttpClient client = new HttpClient();
            string result = await client.GetStringAsync(url);
            if(string.IsNullOrWhiteSpace(result))
                throw new TestFailed("No Health Content eas Returned");

            Console.WriteLine(" Success");
        }

        private static async Task PingTest(Uri ip)
        {
            Console.Write("Ping Test:");
            var result = await new Ping().SendPingAsync(ip.Host, ip.Port);

            if (result?.Status == IPStatus.Success) Console.WriteLine(" Success");
            else throw new TestFailed($"Ping Failed: {result?.Status}");
        }
    }
}
