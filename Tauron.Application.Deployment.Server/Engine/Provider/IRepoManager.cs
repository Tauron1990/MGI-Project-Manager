using System.Threading.Tasks;
using Tauron.Application.Deployment.Server.Data;
using Tauron.Application.SoftwareRepo;

namespace Tauron.Application.Deployment.Server.Engine.Provider
{
    public interface IRepoManager
    {
        RepositoryProvider[] Providers { get; }

        Task Register(string name, string provider, string source);

        Task<SoftwareRepository?> Get(string name);

        Task SyncAll();
    }
}