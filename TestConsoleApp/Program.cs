using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TestConsoleApp
{
    internal class TestService : IHostedService
    {
        private readonly IServiceScopeFactory _factory;

        public TestService(IServiceScopeFactory factory)
        {
            _factory = factory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using var test = _factory.CreateScope();
            using var test2 = test.ServiceProvider.CreateScope();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    internal class Program
    {
        private static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args).ConfigureServices(sc => sc.AddHostedService<TestService>()).RunConsoleAsync();
        }
    }
}