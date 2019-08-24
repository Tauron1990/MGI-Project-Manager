using System.Threading.Tasks;
using CQRSlite.Commands;
using CQRSlite.Domain;
using Tauron.CQRS.Services.Extensions;

namespace EventDeliveryTest.Test
{
    [CQRSHandler]
    public class TestCommandHandler : ICommandHandler<TestCommand>
    {
        private readonly ISession _session;

        public TestCommandHandler(ISession session)
        {
            _session = session;
        }

        public async Task Handle(TestCommand message)
        {
            var aggregate = new TestAggregate();
            aggregate.SetLastValue(message.Parameter);
            
            await _session.Add(aggregate);
            await _session.Commit();
        }
    }
}