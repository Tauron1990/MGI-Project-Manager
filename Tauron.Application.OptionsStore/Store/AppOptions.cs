using System.Threading.Tasks;
using Tauron.Application.OptionsStore.Data;

namespace Tauron.Application.OptionsStore.Store
{
    public sealed class AppOptions : IAppOptions
    {
        private readonly IDataClient _dataClient;
        private IOptionDataCollection _dataCollection;

        public AppOptions(IDataClient dataClient, string name)
        {
            _dataClient = dataClient;
            Name = name;
        }

        public void Init() 
            => _dataCollection = _dataClient.GetCollection(Name);

        public string Name { get; }

        public async Task<IOption> GetOption(string name)
        {
            var pair = await _dataCollection.GetOption(name);

            return new OptionImpl(pair.Key, pair.Value, _dataCollection.Update);
        }

        public async Task DeleteOption(string name) 
            => await _dataCollection.DeleteOption(name);
    }
}