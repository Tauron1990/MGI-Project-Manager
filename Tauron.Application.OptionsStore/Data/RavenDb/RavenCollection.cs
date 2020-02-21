using System;
using System.Threading.Tasks;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Tauron.Application.OptionsStore.Data.RavenDb
{
    public class RavenCollection : IOptionDataCollection
    {
        private const string DatabaseName = "OptionStore";

        private readonly string _prefix;
        private readonly Lazy<IDocumentStore> _store;

        public RavenCollection(string prefix, Lazy<IDocumentStore> store)
        {
            _prefix = prefix;
            _store = store;
        }

        public async Task<OptionsPair> GetOptionAsync(string key)
        {
            using var session = _store.Value.OpenAsyncSession(GetSessionOptions(false));
            var ent = await session.LoadAsync<RavenOptionData>(GetId(key));
            return ent == null ? new OptionsPair(GetId(key), string.Empty) : new OptionsPair(ent.Id, ent.Value);
        }

        public async Task DeleteOptionAsync(string key)
        {
            using var session = _store.Value.OpenAsyncSession(GetSessionOptions());
            session.Delete(GetId(key));
            await session.SaveChangesAsync();
        }

        public async Task UpdateAsync(OptionsPair pair)
        {
            using var session = _store.Value.OpenAsyncSession(GetSessionOptions());
            var ent = await session.LoadAsync<RavenOptionData>(pair.Key);
            if (ent != null)
                ent.Value = pair.Value;
            else
                await session.StoreAsync(new RavenOptionData {Id = pair.Key, Value = pair.Value});

            await session.SaveChangesAsync();
        }

        public OptionsPair GetOption(string key)
        {
            using var session = _store.Value.OpenSession(GetSessionOptions(false));
            var ent = session.Load<RavenOptionData>(GetId(key));
            return ent == null ? new OptionsPair(GetId(key), string.Empty) : new OptionsPair(ent.Id, ent.Value);
        }

        public void DeleteOption(string key)
        {
            using var session = _store.Value.OpenSession(GetSessionOptions());
            session.Delete(GetId(key));
            session.SaveChanges();
        }

        public void Update(OptionsPair pair)
        {
            using var session = _store.Value.OpenSession(GetSessionOptions());
            var ent = session.Load<RavenOptionData>(pair.Key);
            if (ent != null)
                ent.Value = pair.Value;
            else
                session.Store(new RavenOptionData {Id = pair.Key, Value = pair.Value});

            session.SaveChanges();
        }

        private string GetId(string name)
        {
            if (name.Contains("-"))
                throw new ArgumentException(" \"- \" in Names are not allowed");
            return $"options/{_prefix}-{name}";
        }

        private SessionOptions GetSessionOptions(bool tracking = true)
        {
            return new SessionOptions
            {
                Database = DatabaseName,
                NoTracking = !tracking
            };
        }
    }
}