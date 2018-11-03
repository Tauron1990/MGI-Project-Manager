using System.Linq;
using JetBrains.Annotations;

namespace Tauron.Application.Common.BaseLayer.Data
{
    [PublicAPI]
    public interface IRepository<TEntity, TKey>
        where TEntity : class
    {
        IQueryable<TEntity> Query();
        IQueryable<TEntity> QueryAsNoTracking();

        TEntity Find(TKey key);
        void Update(TEntity entity);
        void Remove(TEntity entity);
        void Add(TEntity entity);
    }
}