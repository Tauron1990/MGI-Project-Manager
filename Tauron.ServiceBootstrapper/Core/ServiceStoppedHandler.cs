using System.Threading.Tasks;
using CQRSlite.Events;
using Microsoft.Extensions.Options;
using ServiceManager.CQRS;
using Tauron.CQRS.Common.Configuration;
using Tauron.CQRS.Services.Extensions;

namespace Tauron.ServiceBootstrapper.Core
{
    [CQRSHandler]
    public sealed class ServiceStoppedHandler : IEventHandler<ServiceStoppedEvent>
    {
        private readonly IOptions<ClientCofiguration> _options;

        public ServiceStoppedHandler(IOptions<ClientCofiguration> options) => _options = options;

        public Task Handle(ServiceStoppedEvent message)
        {
            if(message.ServiceName == _options.Value.ServiceName)
                BootStrapper.Shutdown();

            return Task.CompletedTask;
        }
    }
}