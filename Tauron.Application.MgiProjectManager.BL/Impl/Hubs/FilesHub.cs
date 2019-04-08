using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.SignalR;
using Tauron.Application.MgiProjectManager.BL.Contracts.Hubs;

namespace Tauron.Application.MgiProjectManager.BL.Impl.Hubs
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public class FilesHub : Hub<IFilesHub>
    {
    }
}
