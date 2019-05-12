using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Tauron.MgiProjectManager.Identity
{
    public class Program
    {
        static readonly EventId InitDatabase = new EventId(101, "Error whilst creating and seeding database");

        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();


            //Seed database
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var databaseInitializer = services.GetRequiredService<DatabaseInitializer>();
                    databaseInitializer.SeedAsync(true).Wait();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogCritical(InitDatabase, ex, InitDatabase.Name);

                    throw new Exception(InitDatabase.Name, ex);
                }
            }

            host.Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                                          {
                                              webBuilder.UseIIS();
                                              webBuilder.UseStartup<Startup>();
                                          });
    }
}
