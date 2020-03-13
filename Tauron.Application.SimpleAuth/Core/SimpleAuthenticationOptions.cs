using Microsoft.AspNetCore.Authentication;

namespace Tauron.Application.SimpleAuth.Core
{
    public class SimpleAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string Realm { get; set; }
    }
}