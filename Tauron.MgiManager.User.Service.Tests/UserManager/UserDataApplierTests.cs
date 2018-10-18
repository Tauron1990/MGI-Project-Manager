using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tauron.CQRS.Services.Core;
using Tauron.MgiManager.User.Service.Data;
using Tauron.MgiManager.User.Service.UserManager;
using Tauron.MgiManager.User.Shared;
using Tauron.MgiManager.User.Shared.Events;
using Tauron.TestHelper.Mocks;
using Xunit;
using Xunit.Abstractions;

namespace Tauron.MgiManager.User.Service.Tests.UserManager
{
    public class UserDataApplierTests
    {
        private readonly ILogger<UserDataApplier> _logger;

        public UserDataApplierTests(ITestOutputHelper outputHelper) 
            => _logger = new MockLogger<UserDataApplier>(outputHelper);


        [Fact]
        public async Task Test_UserDataApplier_NewUser()
        {
            var builder = new DbContextOptionsBuilder<UserDatabase>();
            builder.UseInMemoryDatabase(nameof(UserDatabase));
            
            await using var database = new UserDatabase(builder.Options);
            var id = IdGenerator.Generator.NewGuid(UserNamespace.UserNameSpace, "TestUser");
            var user = new UserCreatedEvent("TestUser", "jjjjjjj", Guid.NewGuid().ToString(), id);

            var handler = new UserDataApplier(database, _logger);


            await handler.Handle(user);


            var userEntity = Assert.Single(database.Users);
            Assert.Equal(user.Id, userEntity?.Id);
            Assert.Equal(user.Name, userEntity?.Name);
            Assert.Equal(user.Hash, userEntity?.Password);
            Assert.Equal(user.Salt, userEntity?.Salt);
        }
    }
}