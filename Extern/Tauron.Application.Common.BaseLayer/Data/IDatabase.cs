using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tauron.Application.Common.BaseLayer.Data
{
    public interface IDatabase : IDatabaseIdentifer
    {
        void Remove<TEntity>(TEntity entity)
            where TEntity : BaseEntity;

        void Update<TEntity>(TEntity entity)
            where TEntity : BaseEntity;

        IQueryable<TEntity> Query<TEntity>()
            where TEntity : BaseEntity;

        void Add<TEntity>(TEntity entity)
            where TEntity : BaseEntity;

        void SaveChanges();

        TEntity Find<TEntity, TKey>(TKey key)
            where TEntity : GenericBaseEntity<TKey>;

        Task SaveChangesAsync(CancellationToken cancellationToken);
        void AddRange<TEntity>(IEnumerable<TEntity> newEntities);
    }
}