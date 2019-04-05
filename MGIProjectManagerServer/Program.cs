using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tauron.Application.MgiProjectManager.BL.Contracts;

namespace MGIProjectManagerServer
{
    public class Program
    {
        

        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var serviceProvider = services.GetRequiredService<IServiceProvider>();
                    var configuration = services.GetRequiredService<IConfiguration>();

                    Seed.CreateRoles(serviceProvider, configuration);
                }
                catch (Exception e)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(e, "An error occurrend while creating roles");
                }
            }

            try
            {
                host.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseIIS()
                .UseStartup<Startup>()
                .ConfigureLogging(l =>
                {
                    l.AddEventSourceLogger();
                    l.AddConsole();
                #if DEBUG
                l.AddDebug();
                #endif
            });
        }
    }
}