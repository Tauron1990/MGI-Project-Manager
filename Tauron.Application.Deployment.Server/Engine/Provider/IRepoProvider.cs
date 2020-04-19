using System.Threading.Tasks;
using Tauron.Application.Deployment.Server.Engine.Data;

namespace Tauron.Application.Deployment.Server.Engine.Provider
{
    public interface IRepoProvider
    {
        Task Delete(RegistratedRepositoryEntity repository); 
        Task Init(RegistratedRepositoryEntity repository);

        Task Sync(RegistratedRepositoryEntity repository);
    }
}