using Microsoft.AspNetCore.SignalR;
using Tauron.MgiProjectManager.Dispatcher.Model;

namespace Tauron.MgiProjectManager.Server.Hubs
{
    public class EventTokenImpl<THub, TInterfaceHub> : TypedEventToken<TInterfaceHub>
    where THub : Hub<TInterfaceHub> where TInterfaceHub : class
    {
        private readonly IHubContext<THub, TInterfaceHub> _hubContext;
        public override TInterfaceHub Hub => _hubContext.Clients.All;

        public EventTokenImpl(IHubContext<THub, TInterfaceHub> hubContext) 
            => _hubContext = hubContext;
    }
}