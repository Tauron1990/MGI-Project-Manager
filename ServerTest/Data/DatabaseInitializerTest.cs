using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ServerTest.TestHelper;
using Tauron.MgiProjectManager.Data;
using Tauron.MgiProjectManager.Data.Contexts;
using Tauron.MgiProjectManager.Data.Core;
using Tauron.MgiProjectManager.Dispatcher;
using Xunit;

namespace ServerTest.Data
{
    public class DatabaseInitializerTest
    {
        private static TestingObject<DatabaseInitializer> GetTestingObject()
        {
            var obj = new TestingObject<DatabaseInitializer>();
            var accountMock = new Mock<IAccountManager>();
            accountMock.Setup(am => am.GetRoleByNameAsync("")).ReturnsAsync(() => null);


            obj.AddContextDependecy(options => new ApplicationDbContext(options));
            obj.AddContextDependecy(options => new PersistedGrantDbContext((DbContextOptions<PersistedGrantDbContext>) options, new OperationalStoreOptions()));
            obj.AddContextDependecy(options => new ConfigurationDbContext((DbContextOptions<ConfigurationDbContext>) options, new ConfigurationStoreOptions()));
            obj.AddDependency(accountMock);
            obj.AddDependency(new Mock<ILogger<DatabaseInitializer>>());
            obj.AddDependency(new Mock<ITimedTaskManager>());

            return obj;
        }

        [Fact(DisplayName = "Database Initalizer: Check Correct Inital Data")]
        public async Task SeedAsync_InitalData()
        {
            var test = GetTestingObject();

            var initialize = test.GetResolvedTestingObject();

            await initialize.SeedAsync(false);


        }
    }
}