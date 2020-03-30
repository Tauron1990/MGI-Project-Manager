using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Tauron.Application.Deployment.Server.Engine.Provider;
using Tauron.Application.Logging;

namespace Tauron.Application.Deployment.Server
{
    public sealed class SyncService : BackgroundService
    {
        private readonly Timer _timer;
        private CancellationTokenRegistration _cancellationTokenRegistration;

        public SyncService(IRepoManager manager, ISLogger<SyncService> logger)
        {
            _timer = new Timer((state =>
            {
                try
                {
                    logger.Information("Syncing Repositorys");

                    manager.SyncAll();
                }
                catch (Exception e)
                {
                    logger.Error(e, "Error while Syncing Repositorys");
                }
            }));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer.Change(TimeSpan.Zero, TimeSpan.FromDays(1));
            _cancellationTokenRegistration = stoppingToken.Register(() =>
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
            });

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _cancellationTokenRegistration.Dispose();
            _timer.Dispose();
            base.Dispose();
        }
    }
}