using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tauron.Application.Data.Raven.Impl
{
    public abstract class SessionBase : IDatabaseSession
    {
        private readonly ReaderWriterLockSlim _locker;

        protected SessionBase(ReaderWriterLockSlim locker)
            => _locker = locker;

        public virtual void Dispose()
            => _locker.ExitReadLock();

        public abstract Task<T> LoadAsync<T>(string id);
        public abstract Task SaveChangesAsync();
        public abstract IQueryable<T> Query<T>();
        public abstract void Delete(string id);
        public abstract Task StoreAsync<T>(T data);

        public void Enter()
            => _locker.EnterReadLock();
    }
}