using System.Threading;
using System.Threading.Tasks;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Tauron.Application.Data.Raven.Impl
{
    public sealed class RavenSession : SessionBase
    {
        private readonly IAsyncDocumentSession _session;

        public RavenSession(IDocumentStore store, bool noTracking, ReaderWriterLockSlim locker)
            : base(locker) =>
            _session = store.OpenAsyncSession(new SessionOptions
                                              {
                                                  NoTracking = noTracking
                                              });

        public override void Dispose()
        {
            _session?.Dispose();
            base.Dispose();
        }

        public override Task<T> LoadAsync<T>(string id) => _session.LoadAsync<T>(id);

        public override Task SaveChangesAsync() => _session.SaveChangesAsync();

        public override void Delete(string id) => _session.Delete(id);

        public override Task StoreAsync<T>(T data) => _session.StoreAsync(data);
    }
}