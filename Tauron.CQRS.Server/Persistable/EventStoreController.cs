using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tauron.CQRS.Common.ServerHubs;
using Tauron.CQRS.Server.Core;
using Tauron.CQRS.Server.EventStore;
using Tauron.CQRS.Server.EventStore.Data;

namespace Tauron.CQRS.Server.Persistable
{
    [ApiController]
    [Route("Api/[controller]")]
    public class EventStoreController : ControllerBase
    {
        private readonly DispatcherDatabaseContext _context;
        private readonly IApiKeyStore _store;

        public EventStoreController(DispatcherDatabaseContext context, IApiKeyStore store)
        {
            _context = context;
            _store = store;
        }

        [Route("AddEvents")]
        [HttpGet]
        public async Task<ActionResult<bool>> AddEvents(IEnumerable<DomainEvent> events, string apiKey)
        {
            if (!await _store.Validate(apiKey)) return Forbid();

            await _context.EventEntities.AddRangeAsync(events.Select(e => new EventEntity
            {
                Data = e.EventData,
                EventName = e.EventName,
                EventStatus = EventStatus.Pending,
                EventType = e.EventType,
                Id = e.Id,
                Origin = e.TypeName,
                Version = e.Version,
                TimeStamp = e.TimeStamp
            }));

            await _context.SaveChangesAsync();

            return true;
        }

        [Route("GetEvents")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DomainEvent>>> GetEvents(Guid aggregateId, int fromVersion, string apiKey)
        {
            if (!await _store.Validate(apiKey)) return Forbid();

            return _context.EventEntities.Where(ee => ee.Id == aggregateId).Where(ee => ee.Version > fromVersion)
                .Select(ee => new DomainEvent
                {
                    EventData = ee.Data,
                    EventName = ee.EventName,
                    Id = ee.Id,
                    TypeName = ee.Origin,
                    EventType = ee.EventType,
                    Version = ee.Version
                }).ToList();
        }
    }
}