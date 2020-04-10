using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace JKang.IpcServiceFramework.IntegrationTests
{
    public class EdgeCaseTest : IDisposable
    {
        public EdgeCaseTest()
        {
            // configure DI
            var services = new ServiceCollection()
               .AddIpc(builder => builder.AddNamedPipe().AddService<ITestService, TestService>());
            _port = new Random().Next(10000, 50000);
            var host = new IpcServiceHostBuilder(services.BuildServiceProvider())
               .AddTcpEndpoint<ITestService>(
                    Guid.NewGuid().ToString(),
                    IPAddress.Loopback,
                    _port)
               .Build();
            _cancellationToken = new CancellationTokenSource();
            host.RunAsync(_cancellationToken.Token);

            _client = new IpcServiceClientBuilder<ITestService>()
               .UseTcp(IPAddress.Loopback, _port)
               .Build();
        }

        public void Dispose()
        {
            _cancellationToken.Cancel();
        }

        private readonly CancellationTokenSource _cancellationToken;
        private readonly int _port;
        private readonly IpcServiceClient<ITestService> _client;

        [Fact]
        public async Task HugeMessage()
        {
            var buffer = new byte[100000000]; // 100MB
            new Random().NextBytes(buffer);
            var result = await _client.InvokeAsync(x => x.ReverseBytes(buffer));
        }
    }
}