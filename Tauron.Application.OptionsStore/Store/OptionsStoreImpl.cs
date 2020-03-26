using Tauron.Application.OptionsStore.Data;

namespace Tauron.Application.OptionsStore.Store
{
    public class OptionsStoreImpl : IOptionsStore
    {
        private readonly IDataClient _dataClient;

        public OptionsStoreImpl(IDataClient dataClient) => _dataClient = dataClient;

        public IAppOptions GetAppOptions(string applicationName)
        {
            var opt = new AppOptions(_dataClient, applicationName);
            opt.Init();

            return opt;
        }
    }
}