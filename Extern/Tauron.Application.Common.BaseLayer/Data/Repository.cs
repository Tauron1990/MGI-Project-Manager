using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Tauron.Application.Common.BaseLayer.Data
{
    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : GenericBaseEntity<TKey>
    {
        private readonly IDatabase _database;

        public Repository(IDatabase database)
        {
            _database = database;
        }

        public IQueryable<TEntity> Query()
        {
            return _database.Query<TEntity>();
        }

        public IQueryable<TEntity> QueryAsNoTracking()
        {
            return Query().AsNoTracking();
        }

        public TEntity Find(TKey key)
        {
            return _database.Find<TEntity, TKey>(key);
        }

        public void Update(TEntity entity)
        {
            _database.Update(entity);
        }

        public void Remove(TEntity entity)
        {
            _database.Remove(entity);
        }

        public void Add(TEntity entity)
        {
            _database.Add(entity);
        }
    }
}