using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Tauron.MgiProjectManager.Dispatcher
{
    public class MySchemeHandler : IAuthenticationHandler
    {
        public Task<AuthenticateResult> AuthenticateAsync() => Task.FromResult(AuthenticateResult.NoResult());

        public Task ChallengeAsync(AuthenticationProperties properties) => Task.CompletedTask;

        public Task ForbidAsync(AuthenticationProperties properties) => Task.CompletedTask;

        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context) => Task.CompletedTask;
    }
}