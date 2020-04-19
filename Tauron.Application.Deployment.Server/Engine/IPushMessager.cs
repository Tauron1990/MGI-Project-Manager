using System;
using System.Threading.Tasks;
using Tauron.Application.Deployment.Server.Services;

namespace Tauron.Application.Deployment.Server.Engine
{
    public interface IPushMessager
    {
        event Func<SyncError, Task> OnError;

        Task SyncError(string repoName, string errorInfo);
    }
}