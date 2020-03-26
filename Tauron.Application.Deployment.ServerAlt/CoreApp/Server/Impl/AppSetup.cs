using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tauron.Application.Deployment.Server.CoreApp.Server.Impl
{
    public sealed class AppSetup : IAppSetup
    {
        private readonly DatabaseOptions _optionsStore;
        private string? _currentId;

        private int _isInit = 1;

        public AppSetup(DatabaseOptions optionsStore)
            => _optionsStore = optionsStore;

        public bool IsFinish { get; private set; }

        public async Task Init()
        {
            if (Interlocked.CompareExchange(ref _isInit, 0, 1) == 1)
                IsFinish = await _optionsStore.GetIsSetupFinisht();
        }

        public string GetNewId()
        {
            _currentId = Guid.NewGuid().ToString("D");
            return _currentId;
        }

        public bool InvalidateId(string id)
        {
            if (string.IsNullOrEmpty(_currentId))
                return false;
            if (_currentId == id)
            {
                _currentId = null;
                return true;
            }

            return false;
        }
    }
}