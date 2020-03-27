using System;
using Microsoft.Extensions.Options;

namespace Tauron.Application.SimpleAuth.Core
{
    public class SimpleAuthenticationPostConfigureOptions : IPostConfigureOptions<SimpleAuthenticationOptions>
    {
        private readonly IOptions<SimplAuthSettings> _settings;

        public SimpleAuthenticationPostConfigureOptions(IOptions<SimplAuthSettings> settings) 
            => _settings = settings;

        public void PostConfigure(string name, SimpleAuthenticationOptions options)
        {
            if (options.TokenTimeout.TotalSeconds < 60)
                throw new InvalidOperationException("Token Timeout is to Low");

            if (!string.IsNullOrEmpty(options.Realm)) return;

            options.Realm = _settings.Value.AppName;

            if(string.IsNullOrEmpty(options.Realm))
                throw new InvalidOperationException("Realm must be provided in options");
        }
    }
}