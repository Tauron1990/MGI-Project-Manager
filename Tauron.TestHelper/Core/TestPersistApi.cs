using System.Linq;
using System.Threading.Tasks;
using Tauron.CQRS.Common.Dto;
using Tauron.CQRS.Common.Dto.Persistable;
using Tauron.CQRS.Services.Core;
using Tauron.TestHelper.Data;

namespace Tauron.TestHelper.Core
{
    public class TestPersistApi : IPersistApi
    {
        private readonly DataStore _store;

        public TestPersistApi(DataStore store) 
            => _store = store;

        public Task<ObjectStade> Get(ApiObjectId id) 
            => Task.FromResult(_store.ObjectStades.FirstOrDefault(os => os.Identifer == id.Id));

        public async Task Put(ApiObjectStade stade)
        {
            await _store.AddAsync(stade.ObjectStade);
            await _store.SaveChangesAsync();
        }
    }
}