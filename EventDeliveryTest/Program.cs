using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using CQRSlite.Queries;
using EventDeliveryTest.Test;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Tauron.CQRS.Common.Configuration;
using Tauron.CQRS.Services;
using Tauron.CQRS.Services.Extensions;
using Tauron.ServiceBootstrapper;

namespace EventDeliveryTest
{
    class TestFailed : Exception
    {
        public TestFailed(string msg)
        : base(msg)
        {
            
        }
    }

    static class Program
    {
        private const string Msg = "Hallo-Welt";

        private static IConfiguration _configuration;

        private static async Task Main(string[] args)
        {
            await BootStrapper.Run<Empty>(args, startUp: async (sp, so) =>
                                                  {
                                                      var logger = new LoggerConfiguration()
                                                         .WriteTo.Console(outputTemplate: "{Message:lj}{NewLine}{Exception}")
                                                         .WriteTo.File("Test.log").CreateLogger();

                                                      try
                                                      {
                                                          Console.Title = "Event Delivery Test";

                                                          Console.WriteLine("---Welcome To Event Delivery Test---");
                                                          Console.WriteLine("Please Enter The IP of the Server to Test:");
                                                          Console.Write("IP: ");
                                                          //_configuration = GetConfiguration();

                                                          //Console.WriteLine("http://localhost:54005");
                                                          //Uri ip = new Uri("http://localhost:54005");// Console.ReadLine();

                                                          //http://192.168.105.18:81
                                                          Console.WriteLine("http://192.168.105.18:81");
                                                          var ip = new Uri(sp.GetRequiredService<IOptions<ClientCofiguration>>().Value.BaseUrl);
                                                          //new Uri(_configuration.GetValue<string>("Dispatcher")); // Console.ReadLine();

                                                          Console.WriteLine();
                                                          //Console.WriteLine("Press Enter to Start...");
                                                          //Console.ReadKey();
                                                          Console.WriteLine();

                                                          Console.WriteLine("---Tests---");
                                                          try
                                                          {
                                                              Console.WriteLine();

                                                              await PingTest(ip);
                                                              await HealthTest(ip);
                                                              //var temp = ServiceCreationTest(ip);
                                                              await TestEventDeleivery(sp);
                                                              await TestQuery(sp);

                                                              Console.WriteLine();
                                                              Console.WriteLine("Tests Completed");
                                                          }
                                                          catch (Exception e)
                                                          {
                                                              Console.WriteLine();
                                                              Console.WriteLine();
                                                              Console.WriteLine("Tests Failed:");
                                                              Console.WriteLine(e.Demystify());
                                                              logger.Error(e.Demystify(), "Tests Failed");
                                                          }
                                                      }
                                                      catch (Exception e)
                                                      {
                                                          Console.WriteLine(e);
                                                          logger.Error(e, "Error Init");
                                                      }

                                                  }, clientConfig: c => c.AddAwaiter<TestEvent>());
        }

        private static async Task TestQuery(IServiceProvider serviceProvider)
        {
            Console.Write("Event Query Test:");

            using var scope = serviceProvider.CreateScope();
            var queryProcessor = scope.ServiceProvider.GetRequiredService<IQueryProcessor>();

            var result = await queryProcessor.Query(new TestQueryData());

            if(result == null)
                throw new TestFailed("Query Result was Null");
            if(result.Parameter != Msg)
                throw new TestFailed($"Query Result is Not Correct: {result.Parameter}");

            Console.WriteLine(" Success");
        }

        private static async Task TestEventDeleivery(IServiceProvider serviceProvider)
        {
            Console.Write("Event Delivery Test:");

            using var scope = serviceProvider.CreateScope();

            var awaiter = scope.ServiceProvider.GetRequiredService<AwaiterBase<TestCommand, TestEvent>>();



            var (ok, testEvent) = await awaiter.SendAndAwait(new TestCommand { Parameter = Msg });

            if (!ok || Msg != testEvent.Result) throw new TestFailed($"No Correct repond. Timeout: {!ok}");

            Console.WriteLine(" Success");
        }

        //private static IServiceProvider ServiceCreationTest(Uri ip)
        //{
        //    Console.Write("Service Creation Test:");

        //    IServiceCollection collection = new ServiceCollection();

        //    collection.AddLogging(lb => lb.AddConsole());
        //    collection.AddCQRSServices(c => c
        //                                  .AddFrom<TestAggregate>()
        //                                  .SetUrls(ip, _configuration.GetValue<string>("ServiceName"), _configuration.GetValue<string>("ApiKey"))
        //                                  .AddAwaiter<TestEvent>());

        //    var temp = collection.BuildServiceProvider();
        //    Console.WriteLine(" Success");

        //    return temp;
        //}

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

        //private static IConfiguration GetConfiguration()
        //{
        //    var builder = new ConfigurationBuilder();

        //    builder.AddJsonFile("ServiceSettings.json");

        //    return builder.Build();
        //}
    }
}
