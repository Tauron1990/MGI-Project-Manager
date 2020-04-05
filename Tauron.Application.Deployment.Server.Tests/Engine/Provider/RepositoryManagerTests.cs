using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Neleus.DependencyInjection.Extensions;
using Tauron.Application.Data.Raven;
using Tauron.Application.Deployment.Server.Data;
using Tauron.Application.Deployment.Server.Engine;
using Tauron.Application.Deployment.Server.Engine.Data;
using Tauron.Application.Deployment.Server.Engine.Provider;
using TestHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Tauron.Application.Deployment.Server.Tests.Engine.Provider
{
    public sealed class RepositoryManagerTests
    {
        private readonly ITestOutputHelper _helper;

        public RepositoryManagerTests(ITestOutputHelper helper) 
            => _helper = helper;

        private TestService<IRepositoryManager> CreateTestBase(Func<Mock<IRepoProvider>, Action<Mock<IRepoProvider>>?>? providerConfig = null)
        {
            return ServiceTest.Create<IRepositoryManager, RepositoryManager>(_helper, config: sc =>
            {
                var mock = new Mock<IRepoProvider>();

                var assert = providerConfig?.Invoke(mock);

                sc.AddService(() => mock.Object, s => assert?.Invoke(mock));
                sc.Configure(col =>
                {
                    col.AddOptions<DatabaseOption>().Configure(d => d.Debug = true);
                    col.AddOptions<LocalSettings>().Configure(ls => ls.DatabaseName = "Database-Name");
                    col.AddDataRaven();
                    col.AddByName<IRepoProvider, RepositoryProvider>()
                       .Add<IRepoProvider>("Test", new RepositoryProvider
                                                   {
                                                       Description = "Test Desc",
                                                       Id = "Test",
                                                       Name = "Test 1"
                                                   })
                       .Build();

                });

                sc.CreateMock<IFileSystem>().RegisterMock();
                sc.CreateMock<IPushMessager>().RegisterMock();
            });
        }

        [Fact]
        public async Task RegisterValidTest()
        {
            var test = CreateTestBase(m =>
            {
                m.Setup(rp => rp.Init(It.IsAny<RegistratedReporitoryEntity>())).Returns(Task.CompletedTask);
                m.Setup(rp => rp.Sync(It.IsAny<RegistratedReporitoryEntity>())).Returns(Task.CompletedTask);

                return mm =>
                {
                    mm.Verify(rp => rp.Init(It.IsAny<RegistratedReporitoryEntity>()), Times.Exactly(1));
                    mm.Verify(rp => rp.Sync(It.IsAny<RegistratedReporitoryEntity>()), Times.Exactly(1));
                };
            });

            await test.Run(async s =>
            {
                var (msg, ok) = await s.Register("Test", "Test", "Test", "Test");

                Assert.Null(msg);
                Assert.True(ok);
            });
        }
    }
}