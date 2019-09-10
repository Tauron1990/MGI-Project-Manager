using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;
using Tauron.CQRS.Common.Dto;
using Tauron.CQRS.Common.ServerHubs;

namespace Tauron.CQRS.Services.Core
{
    public interface IEventServerApi
    {
        [Get(nameof(AddEvents))]
        Task<bool> AddEvents([Body]ApiEventMessage eventMessage);

        [Get(nameof(GetEvents))]
        Task<IEnumerable<ServerDomainMessage>> GetEvents([Body]ApiEventId eventId);
    }
}