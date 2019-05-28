using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Tauron.CQRS.Common.Dto.Health;

namespace Tauron.CQRS.Health.Core
{
    public interface IStatisticsTracker
    {
        HealthData GenerateData();

        Task AddRequest(HttpContext context);
    }
}