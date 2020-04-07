using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Tauron.Application.SimpleAuth.Api.Proto;
using Tauron.Application.SimpleAuth.Core;

namespace Tauron.Application.SimpleAuth
{
    public static class Extensions
    {
        public static void AddSimpleAuth(this AuthenticationBuilder builder, Action<SimpleAuthenticationOptions>? options = null)
        {
            builder.Services.TryAddScoped<LoginServiceImpl>();
            builder.Services.TryAddSingleton<IPasswordVault, PasswordVault>();
            builder.Services.TryAddSingleton<ITokenManager, TokenManager>();

            builder.Services.TryAddSingleton<IPostConfigureOptions<SimpleAuthenticationOptions>, SimpleAuthenticationPostConfigureOptions>();
            builder.AddScheme<SimpleAuthenticationOptions, SimpleAuthenticationHandler>("Simple", options);
        }

        public static void AddSimpleAuthApi(this IMvcBuilder builder) 
            => builder.AddApplicationPart(typeof(Extensions).Assembly);

        public static void AddSimpleAuthApi(this IEndpointRouteBuilder builder) 
            => builder.MapGrpcService<LoginServiceImpl>();
    }
}