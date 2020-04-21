using System.Threading.Tasks;
using Tauron.Application.Deployment.Server.Engine.Data;
using Tauron.Application.Files.VirtualFiles;

namespace Tauron.Application.Deployment.Server.Engine.Provider
{
    public interface IRepoProvider
    {
        Task Delete(RegistratedRepositoryEntity repository, IDirectory directory); 

        Task Init(RegistratedRepositoryEntity repository, IDirectory directory);

        Task Sync(RegistratedRepositoryEntity repository, IDirectory directory);
    }
}