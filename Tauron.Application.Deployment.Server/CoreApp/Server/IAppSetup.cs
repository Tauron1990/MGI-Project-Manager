using System.Threading.Tasks;

namespace Tauron.Application.Deployment.Server.CoreApp.Server
{
    public interface IAppSetup
    {
        bool IsFinish { get; }
        Task Init();
        string GetNewId();
        bool InvalidateId(string id);
    }
}