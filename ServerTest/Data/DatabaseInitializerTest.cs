using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Moq;
using ServerTest.TestHelper;
using Tauron.MgiProjectManager.Data;
using Tauron.MgiProjectManager.Data.Contexts;
using Tauron.MgiProjectManager.Data.Core;
using Tauron.MgiProjectManager.Dispatcher;
using Tauron.MgiProjectManager.Model;
using Xunit;
using Xunit.Abstractions;

namespace ServerTest.Data
{
    public class DatabaseInitializerTest : TestBaseClass
    {
        public DatabaseInitializerTest(ITestOutputHelper testOutputHelper) 
            : base(testOutputHelper)
        {
        }

        private TestingObject<DatabaseInitializer> GetTestingObject()
        {
            var obj = new TestingObject<DatabaseInitializer>().
            AddContextDependecy(options => new ApplicationDbContext(options)).
            AddContextDependecy(options => new PersistedGrantDbContext((DbContextOptions<PersistedGrantDbContext>) options, new OperationalStoreOptions())).
            AddContextDependecy(options => new ConfigurationDbContext((DbContextOptions<ConfigurationDbContext>) options, new ConfigurationStoreOptions())).

            AddDependency(new Mock<ITimedTaskManager>()).

            AddDependency(BuildMock(CreateMockBuilder<IAccountManager>(
                m => m.Setup(am => am.GetRoleByNameAsync(""))
                    .ReturnsAsync(() => null),

                m => m.Setup(am => am.CreateRoleAsync(It.IsAny<ApplicationRole>(), It.IsAny<IEnumerable<string>>()))
                    .Returns(() => Task.FromResult((true, Array.Empty<string>()))),

                m => m.Setup(am => am.CreateUserAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>(), It.IsAny<string>()))
                    .Returns(() => Task.FromResult((true, Array.Empty<string>())))
            )));

            AddLogger(obj);

            return obj;
        }

        [Fact(DisplayName = "Database Initalizer: Check Correct Inital Data")]
        public async Task SeedAsync_InitalData()
        {
            var test = GetTestingObject();

            var initialize = test.GetResolvedTestingObject();

            await initialize.SeedAsync(false);

            var context3 = test.GetDependency<ConfigurationDbContext>();

            Assert.True(context3.ApiResources.Any(), "context3.ApiResources.Any()");
            Assert.True(context3.Clients.Any(), "context3.Clients.Any()");
            Assert.True(context3.IdentityResources.Any(), "context3.IdentityResources.Any()");
        }
    }
}