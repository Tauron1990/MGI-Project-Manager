using Microsoft.AspNetCore.SignalR;
using Tauron.Application.MgiProjectManager.BL.Contracts.Hubs;
using Tauron.MgiProjectManager.Dispatcher.Model;

namespace Tauron.MgiProjectManager.Server.Hubs
{
    [Export(typeof(IEventHelper), LiveCycle = LiveCycle.Transistent)]
    public class FilesEventEventToken : IEventHelper
    {
        private readonly IHubContext<FilesHub, IFilesHub> _hub;

        public FilesEventEventToken(IHubContext<FilesHub, IFilesHub> hub) 
            => _hub = hub;

        public EventToken GetEventToken() 
            => new EventTokenImpl<FilesHub, IFilesHub>(_hub);
    }
}