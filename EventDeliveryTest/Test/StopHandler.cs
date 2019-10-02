using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CQRSlite.Commands;
using CQRSlite.Events;
using ServiceManager.CQRS;
using Tauron.CQRS.Services.Extensions;

namespace EventDeliveryTest.Test
{
    [CQRSHandler]
    public class StopHandler : ICommandHandler<StopServiceCommand>
    {
        private readonly IEventPublisher _publisher;

        public StopHandler(IEventPublisher publisher) => _publisher = publisher;

        public async Task Handle(StopServiceCommand message)
        {
            await _publisher.Publish(new ServiceStoppedEvent("Temp"));

            Console.WriteLine("Delay Close");

            await Task.Delay(10_000);

            Process.GetCurrentProcess().Kill();
        }
    }
}