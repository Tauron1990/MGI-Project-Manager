using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Tauron.Application.TooUI
{
    public static class CoreProgram
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
               .UseWpf<MainWindow>(configuration => configuration.WithAppFactory(() => new App()))
               .UseStartUp<Startup>()
               .Build();
            await host.RunAsync();
        }
    }
}