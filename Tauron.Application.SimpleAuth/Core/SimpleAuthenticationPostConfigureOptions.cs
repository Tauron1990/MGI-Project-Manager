using System;
using Microsoft.Extensions.Options;

namespace Tauron.Application.SimpleAuth.Core
{
    public class SimpleAuthenticationPostConfigureOptions : IPostConfigureOptions<SimpleAuthenticationOptions>
    {
        public void PostConfigure(string name, SimpleAuthenticationOptions options)
        {
            if (options.TokenTimeout.TotalSeconds < 60)
                throw new InvalidOperationException("Token Timeout is to Low");

            if (string.IsNullOrEmpty(options.Realm))
                throw new InvalidOperationException("Realm must be provided in options");
        }
    }
}