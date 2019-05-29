using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using Tauron.CQRS.Common.ServerHubs;

namespace Tauron.CQRS.Services.Core
{
    public class ServerEventStore : IEventStore
    {
        private readonly IDispatcherApi _api;

        public ServerEventStore(IDispatcherApi api) 
            => _api = api;

        public async Task Save(IEnumerable<IEvent> events, CancellationToken cancellationToken = new CancellationToken())
        {
            await _api.Save(events.Select(e =>
            {
                var type = EventType.TransistentEvent;
                var eventType = e.GetType();

                if (e is CQRSEvent cqrs)
                {
                    type = cqrs.EventType;
                    if (type == EventType.Command)
                        type = EventType.TransistentEvent;
                }

                return new DomainMessage
                {
                    EventName = eventType.Name,
                    EventData = e,
                    EventType = type
                };
            }).ToList());
        }

        public async Task<IEnumerable<IEvent>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = new CancellationToken()) 
            => (await _api.Get(aggregateId, fromVersion, cancellationToken)).Select(de => (IEvent)de.EventData);
    }
}