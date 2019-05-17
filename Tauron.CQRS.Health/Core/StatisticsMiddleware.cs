using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Tauron.CQRS.Health.Core
{
    public class StatisticsMiddleware : IMiddleware
    {
        private readonly IStatisticsTracker _tracker;

        public StatisticsMiddleware(IStatisticsTracker tracker) 
            => _tracker = tracker;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            await _tracker.AddRequest(context);
            await next(context);
        }
    }
}