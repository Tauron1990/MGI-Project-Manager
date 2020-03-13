using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Tauron.Application.SimpleAuth.Api
{
    [Route("api/Login/V1")]
    [ApiController]
    public class LoginV1Controller : ControllerBase
    {
        public LoginV1Controller()
        {
            
        }
    }
}