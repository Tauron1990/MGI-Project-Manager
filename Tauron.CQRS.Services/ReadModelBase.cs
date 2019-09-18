using System.Threading.Tasks;
using CQRSlite.Queries;
using Tauron.CQRS.Services.Core;

namespace Tauron.CQRS.Services
{
    public abstract class ReadModelBase<TRespond, TQuery> : IReadModel<TRespond, TQuery>
        where TQuery : IQuery<TRespond>
    {
        private readonly IDispatcherClient _client;

        protected ReadModelBase(IDispatcherClient client) => _client = client;

        async Task IReadModel<TRespond, TQuery>.ResolveQuery(TQuery query)
        {
            var result = await Query(query);

            
        }

        protected abstract Task<TRespond> Query(TQuery query);
    }
}