using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Tauron.Application.OptionsStore;

namespace Tauron.Application.Deployment.Server.CoreApp.Services
{
    public sealed class Setup
    {
        private readonly DatabaseOptions _optionsStore;
        private int isInit = 1;

        public bool IsFinish { get; private set; }

        public Setup(DatabaseOptions optionsStore) 
            => _optionsStore = optionsStore;

        public async Task Init()
        {
            if (Interlocked.CompareExchange(ref isInit, 0, 1) == 1) 
                IsFinish = await _optionsStore.GetIsSetupFinisht();
        }
    }
}
