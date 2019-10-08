using System;
using System.Threading.Tasks;
using CQRSlite.Domain;
using CQRSlite.Domain.Exception;
using Tauron.CQRS.Services;
using Tauron.CQRS.Services.Extensions;

namespace EventDeliveryTest.Test
{
    [CQRSHandler]
    public class TestCommandHandler : CommandHandlerBase<TestCommand>
    {
        private readonly ISession _session;

        public TestCommandHandler(ISession session) => _session = session;

        public override async Task Handle(TestCommand message)
        {
            TestAggregate aggregate;

            try
            {
                aggregate = await _session.Get<TestAggregate>(TestAggregate.IdField);
            }
            catch (AggregateNotFoundException)
            {
                aggregate = new TestAggregate();
            }

            aggregate.SetLastValue(message.Parameter);
            
            await _session.Add(aggregate);
            await _session.Commit();
        }
    }
}