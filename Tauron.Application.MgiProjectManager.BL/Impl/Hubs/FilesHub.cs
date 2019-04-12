using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Tauron.Application.MgiProjectManager.BL.Contracts.Hubs;
using Tauron.Application.MgiProjectManager.Server.Data.Core;

namespace Tauron.Application.MgiProjectManager.BL.Impl.Hubs
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    [Authorize(Policy = RoleNames.Viewer)]
    public class FilesHub : Hub<IFilesHub>
    {
    }
}
