using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CQRSlite.Domain;
using Microsoft.Extensions.Logging;
using Tauron.CQRS.Services.Data;
using Tauron.MgiManager.User.Service.UserManager.Roles;
using Tauron.MgiManager.User.Shared.Command;
using Tauron.TestHelper.Mocks;
using Xunit;
using Xunit.Abstractions;

namespace Tauron.MgiManager.User.Service.Tests.UserManager.Roles
{
    public sealed class RoleStoreTests
    {
        private readonly ILogger<RoleStoreHandler> _logger;

        public RoleStoreTests(ITestOutputHelper helper) 
            => _logger = new MockLogger<RoleStoreHandler>(helper);

        [Fact]
        public void Test_Claims_Serialization()
        {
            var testData = new List<string>
                            {
                                "Test1",
                                "Test2",
                                "Test3"
                            };

            var aggregate = new RoleClaimsAggregate(Guid.NewGuid());
            aggregate.Claims.AddRange(testData);

            var serData = aggregate.GetSnapshot().Create();
            var newStade = new AggregateStade();
            newStade.Read(serData);

            aggregate.Restore(newStade);


            Assert.Equal(testData, aggregate.Claims);
        }

        [Theory]
        [InlineData("1D8C40FE-E8E3-4D6C-AFFC-2D60211635E5", "TestData")]
        [InlineData("7043859D-7CC9-4795-880F-4D4A047107B0", "TestData2")]
        public async Task Test_AddClaimToRole_Valid(string id, string data)
        {
            bool commited = false;
            RoleClaimsAggregate aggregate = null;
            var session = new MockSession(new Dictionary<Guid, AggregateRoot>(), add:o => aggregate = o as RoleClaimsAggregate, commit:() => commited = true);
            var testData = new AddClaimToRoleCommand(Guid.Parse(id), data);

            var handler = new RoleStoreHandler(_logger, session);
            await handler.Handle(testData);

            Assert.True(commited);
            Assert.NotNull(aggregate);
            Assert.Single(aggregate.Claims);
            Assert.Equal(id, aggregate.Id.ToString());
        }
    }
}