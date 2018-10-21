using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Tauron.CQRS.Common.ServerHubs;
using Tauron.CQRS.Services.Extensions;
using Tauron.MgiManager.User.Service.Data;
using Tauron.MgiManager.User.Service.Data.Entitys;
using Tauron.MgiManager.User.Service.UserManager.Roles;
using Tauron.MgiManager.User.Shared.Dtos;
using Tauron.MgiManager.User.Shared.Querys;
using Tauron.TestHelper.Mocks;
using Xunit;
using Xunit.Abstractions;

namespace Tauron.MgiManager.User.Service.Tests.UserManager.Roles
{
    public class RoleStoreReadModelTests
    {
        private ILogger<RoleStoreReadModel> _logger;

        public RoleStoreReadModelTests(ITestOutputHelper helper) 
            => _logger = new MockLogger<RoleStoreReadModel>(helper);

        private static UserDatabase GetDatabase(InMemoryDatabaseRoot root = null)
        {
            var builder = new DbContextOptionsBuilder<UserDatabase>();
            builder.UseInMemoryDatabase(nameof(UserDatabase), root);
            var database = new UserDatabase(builder.Options);

            return database;
        }


        [Fact]
        public async Task Test_QueryUserRoleClaims()
        {
            var testRole = Guid.NewGuid();
            var testClaim = Guid.NewGuid();
            var testData = "TestData";

            await using var db = GetDatabase();
            await db.AddAsync(new UserRole(testRole, new List<Claim> {new Claim {ClaimId = testClaim, Data = testData, Id = 1}}));
            await db.SaveChangesAsync();

            UserClaims result = null;

            var dispatcher = new MockDispatcher(sendToClient:(s, message, arg3) => result = message.ToRealMessage<UserClaims>());

            var handler = new RoleStoreReadModel(dispatcher, db, _logger);
            await handler.ResolveQuery(new QueryRoleClaims(testRole), new ServerDomainMessage { Sender = "Test" });

            Assert.NotNull(result);
            Assert.NotNull(result.Claims);

            Assert.Equal(testRole, result.Id);
            var claim = Assert.Single(result.Claims);

            Assert.Equal(testData, claim.Data);
        }
    }
}