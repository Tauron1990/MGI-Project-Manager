using System.Collections.Concurrent;
using MongoDB.Driver;

namespace Tauron.Application.OptionsStore.Data.MongoDb
{
    public class MongoDbDataClient : IDataClient
    {
        private readonly IMongoDatabase _serverDatabase;
        private readonly ConcurrentDictionary<string, IOptionDataCollection> _optionDataCollections  = new ConcurrentDictionary<string, IOptionDataCollection>();

        public MongoDbDataClient(MongoClient serverClient)
        {
            _serverDatabase = serverClient.GetDatabase("OptionsStore");
        }

        public IOptionDataCollection GetCollection(string name) 
            => _optionDataCollections.GetOrAdd(name, k => new MongoDbCollection(_serverDatabase.GetCollection<MongoOption>(k)));
    }
}