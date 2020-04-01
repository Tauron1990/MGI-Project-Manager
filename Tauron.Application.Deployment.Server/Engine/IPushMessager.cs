using System.Threading.Tasks;

namespace Tauron.Application.Deployment.Server.Engine
{
    public interface IPushMessager
    {
        Task SyncError(string repoName, string errorInfo);
    }
}