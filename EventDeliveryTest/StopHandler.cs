using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ServiceManager.CQRS;
using Tauron.CQRS.Services;

namespace EventDeliveryTest
{
    public sealed class StopHandler : CommandHandlerBase<StopServiceCommand>
    {
        public override Task Handle(StopServiceCommand message)
        {
            Process.GetCurrentProcess().Kill();

            return Task.CompletedTask;
        }
    }
}