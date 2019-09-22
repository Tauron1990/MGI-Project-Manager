using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Events;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Tauron.CQRS.Common.ServerHubs;

namespace Tauron.CQRS.Services.Core
{
    [UsedImplicitly]
    public class ServerEventStore : IEventStore
    {
        private readonly IDispatcherApi _api;

        public ServerEventStore(IDispatcherApi api) 
            => _api = api;

        public async Task Save(IEnumerable<IEvent> events, CancellationToken cancellationToken = new CancellationToken())
        {
            await _api.Save(events.Select(e =>
            {
                var type = EventType.Event;
                var eventType = e.GetType();

                // ReSharper disable once InvertIf
                //if (e is CQRSEvent cqrs)
                //{
                //    type = cqrs.EventType;
                //    if (type == EventType.Command)
                //        type = EventType.Event;
                //}

                return new ServerDomainMessage
                {
                    EventName = eventType.Name,
                    TypeName = e.GetType().AssemblyQualifiedName,
                    EventData = JsonConvert.SerializeObject(e),
                    EventType = type,
                    Id = e.Id,
                    TimeStamp = e.TimeStamp,
                    Version = e.Version
                };
            }).ToList());
        }

        public async Task<IEnumerable<IEvent>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = new CancellationToken()) 
            => (await _api.Get(aggregateId, fromVersion, cancellationToken)).Select(de => (IEvent)JsonConvert.DeserializeObject(de.EventData, Type.GetType(de.TypeName)));
    }
}