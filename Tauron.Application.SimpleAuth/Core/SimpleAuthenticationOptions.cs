using System;
using Microsoft.AspNetCore.Authentication;

namespace Tauron.Application.SimpleAuth.Core
{
    public class SimpleAuthenticationOptions : AuthenticationSchemeOptions
    {
        public TimeSpan TokenTimeout { get; set; } = TimeSpan.FromDays(1);

        public string Realm { get; set; } = string.Empty;
    }
}