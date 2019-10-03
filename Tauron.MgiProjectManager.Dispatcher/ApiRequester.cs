﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Tauron.CQRS.Server.Core;

namespace Tauron.MgiProjectManager.Dispatcher
{
    [ApiController]
    [Route("Api/[controller]")]
    public class ApiRequesterController : ControllerBase
    {
        private readonly IApiKeyStore _keyStore;
        private readonly IConfiguration _configuration;

        public ApiRequesterController(IApiKeyStore keyStore, IConfiguration configuration)
        {
            _keyStore = keyStore;
            _configuration = configuration;
        }

        [Route(nameof(RegisterApiKey))]
        [HttpGet]
        public async Task<ActionResult<string>> RegisterApiKey(string serviceName)
        {
            var callingUrl = Request.Headers["Referer"].ToString();
            var isLocal = Url.IsLocalUrl(callingUrl);

            if (!_configuration.GetValue<bool>("FreeAcess") && !isLocal) return Forbid();

            return await _keyStore.Register(serviceName);
        }
    }
}