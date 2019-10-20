using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tauron.MgiManager.User.Service.Data;
using Tauron.MgiManager.User.Service.UserManager.Roles;
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

        private static UserDatabase GetDatabase()
        {

            UserDatabase database = new UserDatabase();
        }

        [Fact]
        public static Task Test_ClaimToRoleAdded_Valid()
        {
            
        }
    }
}