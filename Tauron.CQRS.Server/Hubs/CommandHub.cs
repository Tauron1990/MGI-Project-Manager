using Microsoft.AspNetCore.SignalR;
using Tauron.CQRS.Common.ServerHubs;

namespace Tauron.CQRS.Server.Hubs
{
    public class CommandHub : Hub<ICommandBus>
    {
        
    }
}