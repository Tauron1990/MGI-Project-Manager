
using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tauron.Application.Deployment.Server.Engine.Provider;
using TestHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Tauron.Application.Deployment.Server.Tests
{
    public class SyncServiceTest
    {
        private readonly ITestOutputHelper _output;

        public SyncServiceTest(ITestOutputHelper output)
            => _output = output;

        [Fact]
        public async Task TestRun()
        {
            var test = ServiceTest.Create<SyncService>(_output, config: sc =>
            {
                sc.CreateMock<IRepositoryManager>()
                   .With(m => m.Setup(p => p.SyncAll()).Throws<InvalidOperationException>())
                   .Assert(m => m.Verify(p => p.SyncAll(), Times.Exactly(1)))
                   .RegisterMock();
            });

            await test.Run(async ss =>
            {
                await ss.StartAsync(CancellationToken.None);
                await Task.Delay(1000);
                await ss.StopAsync(CancellationToken.None);
            });
        }
    }
}