using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Tauron.Application.Data.Raven.Impl
{
    public sealed class InMemoryStore
    {
        private static readonly ConcurrentDictionary<Type, Func<object, string>> IdGenerators = new ConcurrentDictionary<Type, Func<object, string>>();

        private readonly ConcurrentDictionary<string, object> _store = new ConcurrentDictionary<string, object>();

        public Task<T> LoadAsync<T>(string id)
        {
            if (_store.TryGetValue(id, out var val) && val is T value)
                return Task.FromResult(value);

            return Task.FromResult<T>(default!);
        }

        public Task SaveChangesAsync()
            => Task.CompletedTask;

        public void Delete(string id)
            => _store.TryRemove(id, out _);

        public Task StoreAsync<T>(T data)
        {
            if (data == null) return Task.CompletedTask;

            _store[FindId(data)] = data;

            return Task.CompletedTask;
        }

        private static string FindId<T>([NotNull] T o)
        {
            if (o == null) throw new ArgumentNullException(nameof(o));

            var key = o.GetType();

            if (IdGenerators.TryGetValue(key, out var gen))
                return gen(o);

            var mem = key.GetProperties().FirstOrDefault(p => p.Name.ToUpper() == "ID");
            if (mem == null)
                throw new InvalidOperationException("No Id Found");

            var parm = Expression.Parameter(typeof(object));
            var block = Expression.Convert(Expression.Property(parm, mem), typeof(string));

            gen = Expression.Lambda<Func<object, string>>(block, parm).Compile();
            IdGenerators[key] = gen;
            return gen(o);
        }
    }
}