using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Tauron.MgiManager.User.Service.Data;
using Tauron.MgiManager.User.Service.UserManager.Roles;
using Tauron.MgiManager.User.Shared.Events;
using Tauron.TestHelper.Mocks;
using Xunit;
using Xunit.Abstractions;

namespace Tauron.MgiManager.User.Service.Tests.UserManager.Roles
{
    public class RoleStoreApplierTests
    {
        private ILogger<RoleStoreApplier> _logger;

        public RoleStoreApplierTests(ITestOutputHelper helper) 
            => _logger = new MockLogger<RoleStoreApplier>(helper);

        private static UserDatabase GetDatabase(InMemoryDatabaseRoot root = null)
        {
            var builder = new DbContextOptionsBuilder<UserDatabase>();
            builder.UseInMemoryDatabase(nameof(UserDatabase), root);
            var database = new UserDatabase(builder.Options);

            return database;
        }

        [Fact]
        public async Task Test_ClaimToRoleAdded_Valid()
        {
            var root = new InMemoryDatabaseRoot();
            var db = GetDatabase(root);

            var testId = Guid.NewGuid();
            var testRole = Guid.NewGuid();
            var testData = "TestData";

            var data = new ClaimToRoleAddedEvent(testId, testData, testRole);

            var handler = new RoleStoreApplier(_logger, db);
            await handler.Handle(data);

            db.Dispose();
            db = GetDatabase(root);

            var role = Assert.Single(db.UserRoles);
            Assert.NotNull(role);
            var claim = Assert.Single(role.Claims);
            Assert.NotNull(claim);

            Assert.Equal(testId, claim.ClaimId);
            Assert.Equal(testRole, role.Id);
            Assert.Equal(testData, claim.Data);
        }
    }
}