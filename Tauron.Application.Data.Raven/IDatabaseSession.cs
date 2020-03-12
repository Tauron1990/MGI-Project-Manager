using System;
using System.Threading.Tasks;

namespace Tauron.Application.Data.Raven
{
    public interface IDatabaseSession : IDisposable
    {
        Task<T> LoadAsync<T>(string id);

        Task SaveChangesAsync();

        void Delete(string id);

        Task StoreAsync<T>(T data);
    }
}