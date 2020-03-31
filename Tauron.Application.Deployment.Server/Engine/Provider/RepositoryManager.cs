using System.Linq;
using System.Threading.Tasks;
using Neleus.DependencyInjection.Extensions;
using Tauron.Application.Deployment.Server.Data;
using Tauron.Application.Logging;
using Tauron.Application.SoftwareRepo;

namespace Tauron.Application.Deployment.Server.Engine.Provider
{
    public class RepositoryManager : IRepoManager
    {
        private readonly IServiceByNameFactoryMeta<IRepoProvider, RepositoryProvider> _factory;
        private readonly ISLogger<RepositoryManager> _logger;

        public RepositoryProvider[] Providers { get; }

        public RepositoryManager(IServiceByNameFactoryMeta<IRepoProvider, RepositoryProvider> factory, ISLogger<RepositoryManager> logger)
        {
            _factory = factory;
            _logger = logger;

            Providers = factory.GetMetadata().ToArray();
        }

        public Task Register(string name, string provider, string source) => throw new System.NotImplementedException();

        public Task<SoftwareRepository?> Get(string name) => throw new System.NotImplementedException();

        public Task SyncAll() => throw new System.NotImplementedException();
    }
}