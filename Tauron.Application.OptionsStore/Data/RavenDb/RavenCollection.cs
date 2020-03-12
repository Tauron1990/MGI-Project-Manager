using System;
using System.Threading.Tasks;
using Tauron.Application.Data.Raven;

namespace Tauron.Application.OptionsStore.Data.RavenDb
{
    public class RavenCollection : IOptionDataCollection
    {
    //    private const string DatabaseName = "OptionStore";

        private readonly string _prefix;
        private readonly Lazy<IDatabaseRoot> _store;

        public RavenCollection(string prefix, Lazy<IDatabaseRoot> store)
        {
            _prefix = prefix;
            _store = store;
        }

        public async Task<OptionsPair> GetOptionAsync(string key)
        {
            using var session = _store.Value.OpenSession();
            var ent = await session.LoadAsync<RavenOptionData>(GetId(key));
            return ent == null ? new OptionsPair(GetId(key), string.Empty) : new OptionsPair(ent.Id, ent.Value);
        }

        public async Task DeleteOptionAsync(string key)
        {
            using var session = _store.Value.OpenSession(false);
            session.Delete(GetId(key));
            await session.SaveChangesAsync();
        }

        public async Task UpdateAsync(OptionsPair pair)
        {
            using var session = _store.Value.OpenSession(false);
            var ent = await session.LoadAsync<RavenOptionData>(pair.Key);
            if (ent != null)
                ent.Value = pair.Value;
            else
                await session.StoreAsync(new RavenOptionData {Id = pair.Key, Value = pair.Value});

            await session.SaveChangesAsync();
        }

        public OptionsPair GetOption(string key) 
            => Task.Run(async () => await GetOptionAsync(key)).Result;

        public void DeleteOption(string key)
            => Task.Run(async () => await DeleteOptionAsync(key)).Wait();

        public void Update(OptionsPair pair)
            => Task.Run(async () => await UpdateAsync(pair)).Wait();

        private string GetId(string name)
        {
            if (name.Contains("-"))
                throw new ArgumentException(" \"- \" in Names are not allowed");
            return $"options/{_prefix}-{name}";
        }
    }
}