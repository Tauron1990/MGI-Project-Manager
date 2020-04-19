using System;
using System.Threading.Tasks;
using Tauron.Application.Deployment.Server.Services;

namespace Tauron.Application.Deployment.Server.Engine.Impl
{
    public sealed class PushMessanger : IPushMessager
    {
        public event Func<SyncError, Task>? OnError;
        public async Task SyncError(string repoName, string errorInfo)
        {
            var action = OnError;
            if(action == null) return;

            await action(new SyncError
                         {
                             Message = errorInfo,
                             RepoName = repoName
                         });
        }
    }
}