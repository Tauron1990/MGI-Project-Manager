using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tauron.CQRS.Server.Core;

namespace Tauron.MgiProjectManager.Dispatcher
{
    [ApiController]
    [Route("Api/[controller]")]
    public class ApiRequesterController : ControllerBase
    {
        private readonly IApiKeyStore _keyStore;

        public ApiRequesterController(IApiKeyStore keyStore) => _keyStore = keyStore;

        [Route(nameof(RegisterApiKey))]
        [HttpGet]
        public async Task<ActionResult<string>> RegisterApiKey(string serviceName)
        {
            var callingUrl = Request.Headers["Referer"].ToString();
            var isLocal = Url.IsLocalUrl(callingUrl);

            if (!isLocal) return Forbid();

            return await _keyStore.Register(serviceName);
        }
    }
}