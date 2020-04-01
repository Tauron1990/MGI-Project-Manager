using Microsoft.Extensions.DependencyInjection;
using Moq;
using Neleus.DependencyInjection.Extensions;
using Tauron.Application.Data.Raven;
using Tauron.Application.Deployment.Server.Data;
using Tauron.Application.Deployment.Server.Engine;
using Tauron.Application.Deployment.Server.Engine.Provider;
using TestHelpers;
using Xunit.Abstractions;

namespace Tauron.Application.Deployment.Server.Tests.Engine.Provider
{
    public sealed class RepositoryManagerTests
    {
        private readonly ITestOutputHelper _helper;

        public RepositoryManagerTests(ITestOutputHelper helper) 
            => _helper = helper;

        private TestService<IRepositoryManager> CreateTestBase()
        {
            return ServiceTest.Create<IRepositoryManager, RepositoryManager>(_helper, config: sc =>
            {
                var mock = new Mock<IRepoProvider>();

                sc.AddService(() => mock.Object);
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
    }
}