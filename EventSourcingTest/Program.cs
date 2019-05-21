using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventFlow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventSourcingTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //EventFlowOptions.New.CreateResolver().
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Worker>();
                });
    }
}
