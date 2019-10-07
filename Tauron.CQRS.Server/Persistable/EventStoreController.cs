using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tauron.CQRS.Common.Dto;
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

        [Route(nameof(AddEvents))]
        [HttpGet]
        public async Task<ActionResult<bool>> AddEvents([FromBody]ApiEventMessage events)
        {
            if (!await _store.Validate(events.ApiKey)) return Forbid();

            await _context.EventEntities.AddRangeAsync(events.DomainMessages.Where(e => e.EventType == EventType.Event).Select(dm => new
            {
                Event = dm.EventData, 
                Message = dm
            }).Select(e => new EventEntity
            {
                Data = e.Event.ToString(),
                EventName = e.Message.EventName,
                EventType = e.Message.EventType ?? EventType.Unkowen,
                Id = e.Message.Id,
                OriginType = e.Message.TypeName,
                Version = e.Message.Version ?? -1,
                TimeStamp = e.Message.TimeStamp ?? DateTimeOffset.Now
            }));

            await _context.SaveChangesAsync();

            return true;
        }

        [Route(nameof(GetEvents))]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServerDomainMessage>>> GetEvents([FromBody]ApiEventId eventId)
        {
            if (!await _store.Validate(eventId.ApiKey)) return Forbid();

            return _context.EventEntities.Where(ee => ee.Id == eventId.Id).Where(ee => ee.Version > eventId.Version)
                .Select(ee => new ServerDomainMessage
                {
                    EventData = ee.Data,
                    EventName = ee.EventName,
                    EventType = ee.EventType,
                    SequenceNumber = ee.SequenceNumber,
                    Id = ee.Id.Value,
                    TimeStamp = ee.TimeStamp,
                    Version = ee.Version
                }).ToList();
        }
    }
}