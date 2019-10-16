using System.Threading.Tasks;
using CQRSlite.Commands;
using CQRSlite.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceManager.CQRS;
using Tauron.CQRS.Common.Configuration;
using Tauron.CQRS.Services.Extensions;

namespace Tauron.ServiceBootstrapper.Core
{
    [CQRSHandler]
    public sealed class ServiceStopHandler : ICommandHandler<StopServiceCommand>
    {
        private readonly IOptions<ClientCofiguration> _options;
        private readonly ILogger<ServiceStopHandler> _logger;
        private readonly IEventPublisher _eventPublisher;

        public ServiceStopHandler(IOptions<ClientCofiguration> options, ILogger<ServiceStopHandler> logger, IEventPublisher eventPublisher)
        {
            _options = options;
            _logger = logger;
            _eventPublisher = eventPublisher;
        }

        public async Task Handle(StopServiceCommand message)
        {
            if (message.Name == _options.Value.ServiceName)
            {
                _logger.LogInformation("Shutdown Service");
                await _eventPublisher.Publish(new ServiceStoppedEvent(message.Name));
                await BootStrapper.Shutdown();
            }
        }
    }
}