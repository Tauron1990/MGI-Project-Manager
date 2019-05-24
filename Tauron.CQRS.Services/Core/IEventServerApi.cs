using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;
using Tauron.CQRS.Common.ServerHubs;

namespace Tauron.CQRS.Services.Core
{
    public interface IEventServerApi
    {
        [Get(nameof(AddEvents))]
        Task<bool> AddEvents([Body]IEnumerable<DomainEvent> events, [Query]string apiKey);

        [Get(nameof(GetEvents))]
        Task<IEnumerable<DomainEvent>> GetEvents([Query]Guid aggregateId, [Query]int fromVersion, [Query]string apiKey);
    }
}