using System.Threading.Tasks;
using Grpc.Core;
using Scrutor;
using Tauron.Application.ToolUI.Login;

namespace Tauron.Application.ToolUi.SimpleAuth
{
    [ServiceDescriptor(typeof(IClientFactory<>))]
    public sealed class ClientFactory<TClient> : IClientFactory<TClient> where TClient : ClientBase
    {
        private readonly InputService _inputService;
        private readonly IClientFabricator<TClient> _fabricator;

        public ClientFactory(InputService inputService, IClientFabricator<TClient> fabricator)
        {
            _inputService = inputService;
            _fabricator = fabricator;
        }

        public async Task<TClient> Create(string host)
        {
            var pass = new 
        }
    }
}