using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Tauron.MgiProjectManager.Identity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
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
