using System.Threading.Tasks;
using CQRSlite.Events;
using ServiceManager.CQRS;
using Tauron.CQRS.Services.Extensions;

namespace ServiceManager.Core
{
    [CQRSHandler]
    public class ServiceStopHandler : IEventHandler<ServiceStoppedEvent>
    {
        public Task Handle(ServiceStoppedEvent message)
        {
            
        }
    }
}