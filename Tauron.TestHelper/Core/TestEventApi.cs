using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tauron.CQRS.Common.ServerHubs;
using Tauron.CQRS.Services;
using Tauron.TestHelper.Data;

namespace Tauron.TestHelper.Core
{
    public class TestEventApi : IDispatcherApi
    {
        private readonly DataStore _store;

        public TestEventApi(DataStore store) => _store = store;

        public async Task Save(IEnumerable<ServerDomainMessage> events)
        {
            await _store.Messages.AddRangeAsync(events);
            await _store.SaveChangesAsync();
        }

        public Task<IEnumerable<ServerDomainMessage>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = new CancellationToken()) 
            => Task.FromResult<IEnumerable<ServerDomainMessage>>(_store.Messages.Where(m => m.Id == aggregateId && m.Version >= fromVersion).ToArray());
    }
}