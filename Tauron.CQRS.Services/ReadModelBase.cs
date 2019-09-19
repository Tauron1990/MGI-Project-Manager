using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Queries;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tauron.CQRS.Common.ServerHubs;
using Tauron.CQRS.Services.Core;

namespace Tauron.CQRS.Services
{
    public abstract class ReadModelBase<TRespond, TQuery> : IReadModel<TRespond, TQuery>
        where TQuery : IQuery<TRespond>
    {
        private readonly IDispatcherClient _client;

        protected ReadModelBase(IDispatcherClient client) => _client = client;

        async Task IReadModel<TRespond, TQuery>.ResolveQuery(TQuery query, ServerDomainMessage rawMessage)
        {
            var result = await Query(query);

            if(result == null) return;

            await _client.SendToClient(rawMessage.Sender, new ServerDomainMessage
            {
                EventName = rawMessage.EventName,
                EventType = EventType.QueryResult,
                EventData = JsonConvert.SerializeObject(new QueryEvent(rawMessage.EventName, JToken.FromObject(result))),
                TypeName = typeof(QueryEvent).AssemblyQualifiedName
            }, CancellationToken.None);
        }

        protected abstract Task<TRespond> Query(TQuery query);
    }
}