using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Tauron.CQRS.Server
{
    public class DispatcherService : BackgroundService
    {
        #region Overrides of BackgroundService

        protected override Task ExecuteAsync(CancellationToken stoppingToken) => throw new System.NotImplementedException();

        #endregion
    }
}