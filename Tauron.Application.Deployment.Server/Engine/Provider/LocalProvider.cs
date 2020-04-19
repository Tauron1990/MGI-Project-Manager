using System.IO;
using System.Threading.Tasks;
using Anotar.Serilog;
using Tauron.Application.Deployment.Server.Engine.Data;
using Tauron.Application.SoftwareRepo;

namespace Tauron.Application.Deployment.Server.Engine.Provider
{
    public sealed class LocalProvider : IRepoProvider
    {
        private readonly IRepoFactory _repoFactory;

        public LocalProvider(IRepoFactory repoFactory) 
            => _repoFactory = repoFactory;

        public Task Delete(RegistratedRepositoryEntity repository)
        {
            return Task.Run(() =>
            {
                LogTo.Information("Delete Repository Folder {RepoPath}", repository.TargetPath);
                File.Delete(repository.TargetPath);
            });
        }

        public async Task Init(RegistratedRepositoryEntity repository)
        {
            LogTo.Information("Creating Software Repository {Name}", repository.Name);
            await _repoFactory.Create(repository.TargetPath);
        }

        public Task Sync(RegistratedRepositoryEntity repository)
        {
            LogTo.Information("Nothing to Sync in a Local Repository");
            return Task.CompletedTask;
        }
    }
}