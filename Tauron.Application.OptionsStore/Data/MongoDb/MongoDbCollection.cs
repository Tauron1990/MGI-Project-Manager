using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Tauron.Application.OptionsStore.Data.MongoDb
{
    public class MongoDbCollection : IOptionDataCollection
    {
        private readonly IMongoCollection<MongoOption> _collection;

        private int _isInit;
        private readonly ConcurrentDictionary<string, MongoOption> _cache = new ConcurrentDictionary<string, MongoOption>();

        public MongoDbCollection(IMongoCollection<MongoOption> collection)
        {
            _collection = collection;
            Init();
        }

        private void Init()
        {
            if (Interlocked.CompareExchange(ref _isInit, 1, 0) != 0) return;
            
            foreach (var mongoOption in _collection.AsQueryable()) 
                _cache.TryAdd(mongoOption.Key, mongoOption);
        }

        public Task<OptionsPair> GetOption(string key)
        {
            return Task.FromResult(_cache.TryGetValue(key, out var mo) 
                ? new OptionsPair(mo.Value, mo.Key) 
                : new OptionsPair(string.Empty, key));
        }

        public async Task DeleteOption(string key)
        {
            if (_cache.TryRemove(key, out var mo)) 
                await _collection.DeleteOneAsync(mo => mo.Key == key);
        }

        public async Task Update(OptionsPair pair)
        {
            var newOption = _cache.AddOrUpdate(
                pair.Key,
                s => new MongoOption {Key = s, Value = pair.Value},
                (s, option) =>
                {
                    option.Value = pair.Value;
                    return option;
                });
            

            if (ObjectId.Empty == newOption.Id)
                await _collection.InsertOneAsync(newOption);
            else
                await _collection.FindOneAndUpdateAsync(mo => mo.Id == newOption.Id, new ObjectUpdateDefinition<MongoOption>(newOption));
        }
    }
}