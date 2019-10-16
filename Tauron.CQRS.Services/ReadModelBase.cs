using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Queries;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Tauron.CQRS.Common.ServerHubs;
using Tauron.CQRS.Services.Core;

namespace Tauron.CQRS.Services
{
    [PublicAPI]
    public abstract class ReadModelBase<TRespond, TQuery> : IReadModel<TRespond, TQuery>
        where TQuery : IQuery<TRespond>
    {
        private readonly IDispatcherClient _client;

        protected ReadModelBase(IDispatcherClient client) => _client = client;

        async Task IReadModel<TRespond, TQuery>.ResolveQuery(TQuery query, ServerDomainMessage rawMessage)
        {
            var result = await Query(query);

            if(result == null) return;

            var data = new QueryEvent<TRespond>(rawMessage.EventName, result);

            await _client.SendToClient(rawMessage.Sender, new ServerDomainMessage
            {
                EventName = data.GetType().FullName,
                EventType = EventType.QueryResult,
                EventData = JsonConvert.SerializeObject(data),
                TypeName = typeof(QueryEvent<TRespond>).AssemblyQualifiedName
            }, CancellationToken.None);
        }

        protected abstract Task<TRespond> Query(TQuery query);
    }
}