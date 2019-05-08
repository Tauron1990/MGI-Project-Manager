﻿using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Tauron.MgiProjectManager.Data;

namespace Tauron.MgiProjectManager.Server
{
    // Swagger IOperationFilter implementation that will decide which api action needs authorization
    [UsedImplicitly]
    internal class AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            // Check for authorize attribute
            var hasAuthorize = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<AuthorizeAttribute>()
                .Any();

            if (!hasAuthorize) return;

            operation.Responses.Add("401", new Response { Description = "Unauthorized" });

            operation.Security = new List<IDictionary<string, IEnumerable<string>>>
            {
                new Dictionary<string, IEnumerable<string>>
                {
                    { "oauth2", new [] { IdentityServerConfig.ApiName} }
                }
            };
        }
    }

}