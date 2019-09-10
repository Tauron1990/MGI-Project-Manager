using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RestEase;
using Tauron.CQRS.Common.Configuration;
using Tauron.CQRS.Common.Dto;
using Tauron.CQRS.Common.ServerHubs;

namespace Tauron.CQRS.Services.Core
{
    public class DispatcherApi : IDispatcherApi
    {
        private readonly IOptions<ClientCofiguration> _configuration;
        private readonly Lazy<IEventServerApi> _eventServerApi;
        
        public DispatcherApi(IOptions<ClientCofiguration> configuration)
        {
            _configuration = configuration;
            var client = new RestClient(configuration.Value.EventServerApiUrl);
            _eventServerApi = new Lazy<IEventServerApi>(() => client.For<IEventServerApi>(), true);
        }

        public async Task Save(IEnumerable<ServerDomainMessage> events)
        {
            var result = await _eventServerApi.Value.AddEvents(new ApiEventMessage
                                                               {
                                                                   DomainMessages = events.ToArray(), ApiKey = _configuration.Value.ApiKey
                                                               });
            if (result)
                return;

            throw new InvalidOperationException("AddEvents Failed");
        }
        
        public Task<IEnumerable<ServerDomainMessage>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = new CancellationToken()) 
            => _eventServerApi.Value.GetEvents(new ApiEventId{Id =aggregateId, Version = fromVersion, ApiKey = _configuration.Value.ApiKey});
    }
}