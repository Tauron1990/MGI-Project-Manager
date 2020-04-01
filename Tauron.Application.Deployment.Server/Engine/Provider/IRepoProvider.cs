using System.Threading.Tasks;
using Tauron.Application.Deployment.Server.Engine.Data;

namespace Tauron.Application.Deployment.Server.Engine.Provider
{
    public interface IRepoProvider
    {
        Task Delete(RegistratedReporitoryEntity reporitory); 
        Task Init(RegistratedReporitoryEntity reporitory);

        Task Sync(RegistratedReporitoryEntity reporitory);
    }
}