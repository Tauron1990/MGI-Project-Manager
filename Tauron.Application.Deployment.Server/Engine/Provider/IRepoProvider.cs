using System.Threading.Tasks;
using Tauron.Application.Deployment.Server.Engine.Data;

namespace Tauron.Application.Deployment.Server.Engine.Provider
{
    public interface IRepoProvider
    {
        Task Init(RegistratedReporitoryEntity reporitoryEntity);

        Task Sync(RegistratedReporitoryEntity reporitoryEntity);
    }
}