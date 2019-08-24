using System;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using EventDeliveryTest.Test;
using Microsoft.Extensions.DependencyInjection;
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
        static async Task Main(string[] args)
        {
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

                Console.WriteLine();
                Console.WriteLine("Tests Completed");
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine("Tests Failed:");
                Console.WriteLine(e);
            }

            Console.ReadKey();
        }

        private static IServiceProvider ServiceCreationTest(Uri ip)
        {
            IServiceCollection collection = new ServiceCollection();

            collection.AddCQRSServices(c => c
                .ScanFrom<Program>()
                .SetUrls(ip, "Temp", "develop")
                .AddAwaiter<TestCommand, TestEvent>(collection));

            return collection.BuildServiceProvider();
        }

        private static async Task HealthTest(Uri url)
        {
            Console.Write("Health Test");
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
