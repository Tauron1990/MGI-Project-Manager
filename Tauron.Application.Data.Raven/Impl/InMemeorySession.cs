using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tauron.Application.Data.Raven.Impl
{
    public sealed class InMemeorySession : SessionBase
    {
        private readonly InMemoryStore _store;

        public InMemeorySession(InMemoryStore store, ReaderWriterLockSlim locker)
            : base(locker) => _store = store;

        public override Task<T> LoadAsync<T>(string id) => _store.LoadAsync<T>(id);

        public override Task SaveChangesAsync() => _store.SaveChangesAsync();
        public override IQueryable<T> Query<T>() => _store.Query<T>();

        public override void Delete(string id) => _store.Delete(id);

        public override Task StoreAsync<T>(T data) => _store.StoreAsync(data);
    }
}