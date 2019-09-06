using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Tauron.CQRS.Common.Configuration;
using Tauron.CQRS.Common.ServerHubs;

namespace Tauron.CQRS.Services.Core
{
    public class DispatcherApi : IDispatcherApi
    {
        private readonly IOptions<ClientCofiguration> _configuration;
        private readonly IEventServerApi _eventServerApi;
        
        public DispatcherApi(IOptions<ClientCofiguration> configuration, IEventServerApi eventServerApi)
        {
            _configuration = configuration;
            _eventServerApi = eventServerApi;
        }

        public async Task Save(IEnumerable<DomainMessage> events) => await _eventServerApi.AddEvents(events);

        public Task<IEnumerable<DomainMessage>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = new CancellationToken()) 
            => _eventServerApi.GetEvents(aggregateId, fromVersion);
    }
}