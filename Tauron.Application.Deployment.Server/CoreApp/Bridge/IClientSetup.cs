using System.Threading.Tasks;
using Tauron.Application.Deployment.Server.CoreApp.Server;

namespace Tauron.Application.Deployment.Server.CoreApp.Bridge
{
    public interface IClientSetup
    {
        bool IsFinish { get; }

        Task<string> TrySetPath(ServerFileMode path);

        Task Init();
        Task<string> GetNewId();
        Task<bool> InvalidateId(string id);
    }
}