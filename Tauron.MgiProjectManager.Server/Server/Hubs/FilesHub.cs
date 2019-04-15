using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.SignalR;
using Tauron.Application.MgiProjectManager.BL.Contracts.Hubs;

namespace Tauron.MgiProjectManager.Server.Hubs
{
    [UsedImplicitly]
    public class FilesHub : Hub<IFilesHub>
    {
    }
}