using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Tauron.Application.SimpleAuth.Core;

namespace Tauron.Application.SimpleAuth
{
    public static class Extensions
    {
        public static void AddSimpleAuth(this AuthenticationBuilder builder, Action<SimpleAuthenticationOptions> options = null)
        {
            builder.Services.TryAddSingleton<IPostConfigureOptions<SimpleAuthenticationOptions>, SimpleAuthenticationPostConfigureOptions>();
            builder.AddScheme<SimpleAuthenticationOptions, SimpleAuthenticationHandler>("Simple", options);
        }

        public static void AddSimpleAuthApi(this IMvcBuilder builder) 
            => builder.AddApplicationPart(typeof(Extensions).Assembly);
    }
}