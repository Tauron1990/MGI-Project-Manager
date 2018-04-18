using System.Linq;

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
    }
}