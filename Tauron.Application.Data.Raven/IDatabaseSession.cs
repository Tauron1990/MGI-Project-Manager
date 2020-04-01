using System;
using System.Linq;
using System.Threading.Tasks;

namespace Tauron.Application.Data.Raven
{
    public interface IDatabaseSession : IDisposable
    {
        Task<T> LoadAsync<T>(string id);

        Task SaveChangesAsync();

        IQueryable<T> Query<T>();

        void Delete(string id);

        Task StoreAsync<T>(T data);
    }
}