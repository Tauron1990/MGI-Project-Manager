using MongoDB.Bson;

namespace Tauron.Application.OptionsStore.Data.MongoDb
{
    public class MongoOption
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public ObjectId Id { get; set; }
    }
}