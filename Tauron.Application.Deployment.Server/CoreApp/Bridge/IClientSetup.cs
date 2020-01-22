using System.Threading.Tasks;

namespace Tauron.Application.Deployment.Server.CoreApp.Bridge
{
    public interface IClientSetup
    {
        bool IsFinish { get; }
        Task Init();
        Task<string> GetNewId();
        Task<bool> InvalidateId(string id);
    }
}