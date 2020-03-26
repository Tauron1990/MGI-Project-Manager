using System;
using System.Threading.Tasks;
using Tauron.Application.OptionsStore.Data;

namespace Tauron.Application.OptionsStore.Store
{
    public sealed class AppOptions : IAppOptions
    {
        private readonly IDataClient _dataClient;
        private IOptionDataCollection _dataCollection = new Dummy();

        public AppOptions(IDataClient dataClient, string name)
        {
            _dataClient = dataClient;
            Name = name;
        }

        public string Name { get; }

        public async Task<IOption> GetOptionAsync(string name)
        {
            var pair = await _dataCollection.GetOptionAsync(name);

            return new OptionImpl(pair.Key, pair.Value, _dataCollection.UpdateAsync, _dataCollection.Update);
        }

        public async Task DeleteOptionAsync(string name)
        {
            await _dataCollection.DeleteOptionAsync(name);
        }

        public IOption GetOption(string name)
        {
            var pair = _dataCollection.GetOption(name);
            return new OptionImpl(pair.Key, pair.Value, _dataCollection.UpdateAsync, _dataCollection.Update);
        }

        public void DeleteOption(string name)
        {
            _dataCollection.DeleteOption(name);
        }

        public void Init()
        {
            _dataCollection = _dataClient.GetCollection(Name);
        }

        public void Dispose()
        {
        }

        private class Dummy : IOptionDataCollection
        {
            public Task<OptionsPair> GetOptionAsync(string key) => throw new NotSupportedException();

            public Task DeleteOptionAsync(string key) => throw new NotSupportedException();

            public Task UpdateAsync(OptionsPair pair) => throw new NotSupportedException();

            public OptionsPair GetOption(string key) => throw new NotSupportedException();

            public void DeleteOption(string key)
            {
                throw new NotSupportedException();
            }

            public void Update(OptionsPair pair)
            {
                throw new NotSupportedException();
            }
        }
    }
}