using System.Threading.Tasks;
using Tauron.Application.Deployment.Server.CoreApp.Server;
using Tauron.Application.Deployment.Server.CoreApp.Server.Impl;

namespace Tauron.Application.Deployment.Server.CoreApp.Bridge.Impl
{
    public sealed class ServerBridgeImpl : IServerBridge
    {
        private sealed class ClientSetupImpl : IClientSetup
        {
            private readonly IAppSetup _appSetup;

            public ClientSetupImpl(IAppSetup appSetup) 
                => _appSetup = appSetup;

            public bool IsFinish => _appSetup.IsFinish;

            public Task Init() 
                => _appSetup.Init();

            public Task<string> GetNewId() 
                => Task.FromResult(_appSetup.GetNewId());

            public Task<bool> InvalidateId(string id) 
                => Task.FromResult(_appSetup.InvalidateId(id));
        }

        public IClientSetup ClientSetup { get; }

        public ServerBridgeImpl(IAppSetup appSetup) 
            => ClientSetup = new ClientSetupImpl(appSetup);
    }
}