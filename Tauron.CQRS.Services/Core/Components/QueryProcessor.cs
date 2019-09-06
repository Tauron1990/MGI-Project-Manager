using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Queries;

namespace Tauron.CQRS.Services.Core.Components
{
    public class QueryProcessor : IQueryProcessor
    {
        private readonly IDispatcherClient _client;

        public QueryProcessor(IDispatcherClient client) 
            => _client = client;

        public Task<TResponse> Query<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = new CancellationToken()) => _client.Query(query, cancellationToken);
    }
}