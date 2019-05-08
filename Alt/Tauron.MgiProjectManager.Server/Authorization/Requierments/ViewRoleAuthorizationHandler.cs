using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Tauron.MgiProjectManager.Data.Core;

namespace Tauron.MgiProjectManager.Server.Authorization
{
    [Export(typeof(IAuthorizationHandler), LiveCycle = LiveCycle.Singleton)]
    public class ViewRoleAuthorizationHandler : AuthorizationHandler<ViewRoleAuthorizationRequirement, string>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ViewRoleAuthorizationRequirement requirement, string roleName)
        {
            if (context.User == null)
                return Task.CompletedTask;

            if (context.User.HasClaim(ClaimConstants.Permission, ApplicationPermissions.ViewRoles) || context.User.IsInRole(roleName))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}