using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Neleus.DependencyInjection.Extensions;
using Tauron.Application.Data.Raven;
using Tauron.Application.Data.Raven.Impl;
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

        private TestService<IRepositoryManager> CreateTestBase(Func<Mock<IRepoProvider>, Action<Mock<IRepoProvider>>?>? providerConfig = null, Action<InMemoryStore> database = null)
        {
            return ServiceTest.Create<IRepositoryManager, RepositoryManager>(_helper, config: sc =>
            {
                var mock = new Mock<IRepoProvider>();
                var store = new InMemoryStore();

                database?.Invoke(store);
                var assert = providerConfig?.Invoke(mock);

                sc.AddService(() => mock.Object, s => assert?.Invoke(mock));
                sc.Configure(col =>
                {
                    col.AddOptions<DatabaseOption>().Configure(d => d.InMemory = true);
                    col.AddOptions<LocalSettings>().Configure(ls =>
                    {
                        ls.DatabaseName = "Database-Name";
                        ls.ServerFileMode = ServerFileMode.ContentRoot;
                    });
                    col.AddDataRaven().AddMemoryStore("Database-Name", store);
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
                    mm.Verify(rp => rp.Init(It.IsAny<RegistratedReporitoryEntity>()), Times.Once);
                    mm.Verify(rp => rp.Sync(It.IsAny<RegistratedReporitoryEntity>()), Times.Once);
                };
            });

            await test.Run(async s =>
            {
                var (msg, ok) = await s.Register("Test", "Test", "Test", "Test");

                Assert.Null(msg);
                Assert.True(ok);
            });
        }

        [Fact]
        public async Task RegisterErrorTest()
        {
            var test = CreateTestBase(m =>
            {
                m.Setup(rp => rp.Init(It.IsAny<RegistratedReporitoryEntity>())).Returns(Task.CompletedTask);
                m.Setup(rp => rp.Sync(It.IsAny<RegistratedReporitoryEntity>())).Throws<InvalidOperationException>();

                return mm =>
                {
                    mm.Verify(rp => rp.Init(It.IsAny<RegistratedReporitoryEntity>()), Times.Once);
                    mm.Verify(rp => rp.Sync(It.IsAny<RegistratedReporitoryEntity>()), Times.Never);
                };
            });

            await test.Run(async s =>
            {
                var (msg, ok) = await s.Register("Test", "Test", "Test", "Test");

                Assert.NotNull(msg);
                Assert.False(ok);
            });
        }

        public enum DeleteTestType
        {
            NoData,
            Data
        }

        [Theory]
        [InlineData(DeleteTestType.Data)]
        [InlineData(DeleteTestType.NoData)]
        public async Task DeleteRepositoryTest(DeleteTestType testType)
        {
            string name = "TestRepo";

            var test = CreateTestBase(providerConfig: m =>
            {
                return ma =>
                {
                    switch (testType)
                    {
                        case DeleteTestType.NoData:
                            m.Verify(rp => rp.Delete(It.IsAny<RegistratedReporitoryEntity>()), Times.Never);
                            break;
                        case DeleteTestType.Data:
                            m.Verify(rp => rp.Delete(It.IsAny<RegistratedReporitoryEntity>()), Times.Once);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(testType), testType, null);
                    }
                };
            }, store =>
            {
                if (testType == DeleteTestType.Data)
                    store.StoreAsync(new RegistratedReporitoryEntity
                    {
                        Id = name,
                        Name = "Name",
                        Provider = "Test",
                        Source = "Test",
                        SyncCompled = true,
                        TargetPath = "Test"
                    });
            });


        }
    }
}