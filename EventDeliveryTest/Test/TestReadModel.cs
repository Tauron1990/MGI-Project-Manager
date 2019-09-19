using System.Threading.Tasks;
using CQRSlite.Domain;
using Tauron.CQRS.Services;
using Tauron.CQRS.Services.Extensions;

namespace EventDeliveryTest.Test
{
    [CQRSHandler]
    public class TestReadModel : ReadModelBase<TestData, TestQueryData>
    {
        private readonly ISession _session;

        public TestReadModel(ISession session, IDispatcherClient client) : base(client) => _session = session;

        protected override async Task<TestData> Query(TestQueryData query)
        {
            var aggregate = await _session.Get<TestAggregate>(TestAggregate.IdField);

            return new TestData(aggregate.LastValue);
        }
    }
}