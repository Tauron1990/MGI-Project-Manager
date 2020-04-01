using System.Threading.Tasks;
using Tauron.Application.Deployment.Server.Data;
using Tauron.Application.SoftwareRepo;

namespace Tauron.Application.Deployment.Server.Engine.Provider
{
    public interface IRepoManager
    {
        RepositoryProvider[] Providers { get; }

        Task<(string? msg, bool ok)> Register(string name, string provider, string source, string comment);

        Task<RegistratedReporitory[]> GetAllRepositorys();

        Task<(SoftwareRepository? repo, string msg)> Get(string name);

        Task SyncAll();
    }
}