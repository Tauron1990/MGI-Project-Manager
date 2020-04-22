using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Tauron.Application.SimpleAuth.Core
{
    public class SimpleAuthenticationHandler : AuthenticationHandler<SimpleAuthenticationOptions>
    {
        private const string AuthorizationHeaderName = "Authorization";
        private const string SimpleSchemeName = "Simple";

        private readonly IPasswordVault _passwordVault;
        private readonly ITokenManager _tokenManager;


        public SimpleAuthenticationHandler(IOptionsMonitor<SimpleAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder,
            ISystemClock clock, IPasswordVault passwordVault, ITokenManager tokenManager)
            : base(options, logger, encoder, clock)
        {
            _passwordVault = passwordVault;
            _tokenManager = tokenManager;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(AuthorizationHeaderName))
            {
                //Authorization header not in request
                return AuthenticateResult.NoResult();
            }

            if (!AuthenticationHeaderValue.TryParse(Request.Headers[AuthorizationHeaderName], out var headerValue))
            {
                //Invalid Authorization header
                return AuthenticateResult.NoResult();
            }

            if (!SimpleSchemeName.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                //Not Basic authentication header
                return AuthenticateResult.NoResult();
            }

            var headerValueBytes = Convert.FromBase64String(headerValue.Parameter);
            var userAndPassword = Encoding.UTF8.GetString(headerValueBytes);
            var parts = userAndPassword.Split(':');
            if (parts.Length != 2) return AuthenticateResult.Fail("Falscher Basic authentication header");
            var type = parts[0];
            var password = parts[1];
            bool isValid;

            switch (type)
            {
                case "token":
                    isValid = _tokenManager.ValidateToken(password);
                    break;
                case "pass":
                    isValid = await _passwordVault.CheckPassword(password);
                    break;
                default:
                    return AuthenticateResult.Fail("Inkompatibler Modus");
            }

            if (!isValid) return AuthenticateResult.Fail("Falsches Passwort oder Token");

            var claims = new[] {new Claim(ClaimTypes.Name, "Administrator")};
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{Options.Realm}\", charset=\"UTF-8\"";
            await base.HandleChallengeAsync(properties);
        }
    }
}