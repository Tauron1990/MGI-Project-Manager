using Microsoft.AspNetCore.Authorization;

namespace Tauron.MgiProjectManager.Server.Authorization
{
    public class UserAccountAuthorizationRequirement : IAuthorizationRequirement
    {
        public UserAccountAuthorizationRequirement(string operationName) => OperationName = operationName;


        public string OperationName { get; }
    }
}