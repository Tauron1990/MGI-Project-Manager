using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tauron.Application.SimpleAuth.Core;

namespace Tauron.Application.SimpleAuth.Api
{
    [Route("api/Login/V1")]
    [ApiController, Authorize(AuthenticationSchemes = "Simple")]
    public class LoginV1Controller : ControllerBase
    {
        private readonly IPasswordVault _passwordVault;
        private readonly ITokenManager _tokenManager;

        public LoginV1Controller(IPasswordVault passwordVault, ITokenManager tokenManager)
        {
            _passwordVault = passwordVault;
            _tokenManager = tokenManager;
        }

        [HttpGet("Token")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public ActionResult<string> GetToken()
        {
            
        }
    }
}