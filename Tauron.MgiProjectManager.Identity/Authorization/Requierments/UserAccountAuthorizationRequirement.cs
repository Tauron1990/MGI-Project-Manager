using Microsoft.AspNetCore.Authorization;

namespace Tauron.MgiProjectManager.Identity.Authorization.Requierments
{
    public class UserAccountAuthorizationRequirement : IAuthorizationRequirement
    {
        public UserAccountAuthorizationRequirement(string operationName) => OperationName = operationName;


        public string OperationName { get; }
    }
}