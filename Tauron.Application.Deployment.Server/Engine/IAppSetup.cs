using System.Threading.Tasks;

namespace Tauron.Application.Deployment.Server.Engine
{
    public interface IAppSetup
    {
        bool IsFinish { get; }

        Task<bool> TrySetServerPath(ServerFileMode path);

        Task Init();
        string GetNewId();
        bool InvalidateId(string id);
    }
}