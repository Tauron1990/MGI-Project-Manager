using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Tauron.CQRS.Common.ServerHubs;
using Tauron.CQRS.Services.Data.Database;

namespace Tauron.CQRS.Services.Core.Components
{
    [UsedImplicitly]
    public class EventServerApi : IEventServerApi
    {
        private readonly EventSourceContext _eventSourceContext;

        public EventServerApi(EventSourceContext eventSourceContext) 
            => _eventSourceContext = eventSourceContext;

        public async Task AddEvents(IEnumerable<DomainMessage> eventMessage)
        {
            await _eventSourceContext.EventEntities.AddRangeAsync(eventMessage.Select(s => new EventEntity
                                                                                           {
                                                                                               Data = s.EventData,
                                                                                               EventName = s.EventName,
                                                                                               EventType = s.EventType,
                                                                                               Id = s.Id,
                                                                                               OriginType = s.TypeName,
                                                                                               TimeStamp = s.TimeStamp,
                                                                                               Version = s.Version
                                                                                           }));

            await _eventSourceContext.SaveChangesAsync();
        }

        public Task<IEnumerable<DomainMessage>> GetEvents(Guid id, int version)
        {
            return Task.FromResult((IEnumerable<DomainMessage>)
                                   _eventSourceContext.EventEntities
                                      .Where(ee => ee.Id      == id)
                                      .Where(ee => ee.Version > version)
                                      .Select(ee => new DomainMessage
                                                    {
                                                        EventData = ee.Data,
                                                        EventName = ee.EventName,
                                                        EventType = ee.EventType,
                                                        SequenceNumber = ee.SequenceNumber,
                                                        Id = ee.Id.Value,
                                                        TimeStamp = ee.TimeStamp,
                                                        Version = ee.Version
                                                    }).ToList());
        }
    }
}