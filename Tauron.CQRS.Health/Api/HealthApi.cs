using Microsoft.AspNetCore.Mvc;
using Tauron.CQRS.Common.Dto.Health;
using Tauron.CQRS.Health.Core;

namespace Tauron.CQRS.Health.Api
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : Controller
    {
        private readonly IStatisticsTracker _tracker;

        public HealthController(IStatisticsTracker tracker) 
            => _tracker = tracker;

        [HttpGet]
        public ActionResult<HealthData> Get() 
            => _tracker.GenerateData();
    }
}