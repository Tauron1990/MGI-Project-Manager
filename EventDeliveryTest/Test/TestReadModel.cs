using System.Threading.Tasks;
using CQRSlite.Domain;
using CQRSlite.Queries;
using Tauron.CQRS.Services;
using Tauron.CQRS.Services.Extensions;

namespace EventDeliveryTest.Test
{
    [CQRSHandler]
    public class TestReadModel : IReadModel<TestData>
    {
        private readonly ISession _session;

        public TestReadModel(ISession session) => _session = session;

        public async Task<TestData> ResolveQuery(IQuery<TestData> query)
        {
            var aggregate = await _session.Get<TestAggregate>(TestAggregate.IdField);

            return new TestData(aggregate.LastValue);
        }
    }
}