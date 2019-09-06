using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tauron.CQRS.Common.ServerHubs;

namespace Tauron.CQRS.Services.Core
{
    public interface IEventServerApi
    {
        Task AddEvents(IEnumerable<DomainMessage> eventMessage);

        Task<IEnumerable<DomainMessage>> GetEvents(Guid id, int version);
    }
}