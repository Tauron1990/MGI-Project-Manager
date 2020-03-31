using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Neleus.DependencyInjection.Extensions;
using Tauron.Application.Data.Raven;
using Tauron.Application.Deployment.Server.Data;
using Tauron.Application.Logging;
using Tauron.Application.SoftwareRepo;

namespace Tauron.Application.Deployment.Server.Engine.Provider
{
    public class RepositoryManager : IRepoManager, IDisposable
    {
        private readonly IDisposable _subscription;


        private readonly IServiceByNameFactoryMeta<IRepoProvider, RepositoryProvider> _factory;
        private readonly ISLogger<RepositoryManager> _logger;
        private IDatabaseRoot _database;

        public RepositoryProvider[] Providers { get; }

        public RepositoryManager(IServiceByNameFactoryMeta<IRepoProvider, RepositoryProvider> factory, ISLogger<RepositoryManager> logger, IDatabaseCache database, 
            IOptionsMonitor<LocalSettings> settings)
        {
            _factory = factory;
            _logger = logger;

            Providers = factory.GetMetadata().ToArray();
            _database = database.Get(settings.CurrentValue.DatabaseName);

            _subscription = settings.OnChange(ls => Interlocked.Exchange(ref _database, database.Get(ls.DatabaseName)));
        }

        public Task<(string? msg, bool)> Register(string name, string provider, string source)
        {
        }

        public Task<SoftwareRepository?> Get(string name) => throw new System.NotImplementedException();

        public Task SyncAll() => throw new System.NotImplementedException();

        public void Dispose() 
            => _subscription.Dispose();
    }
}