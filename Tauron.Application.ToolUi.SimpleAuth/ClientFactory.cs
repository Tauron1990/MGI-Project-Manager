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
                I think you should use the below api in CallInvokerExtensions to forward headers:

            public static CallInvoker Intercept(this CallInvoker invoker, Func<Metadata, Metadata> interceptor)

            You need to create your ForwardingHeaderFunc like this:

        public static Func<Metadata, Metadata> ForwardingHeaderFunc = (Metadata source) =>
        {
            foreach (var httpHeader in _forwardHeadersProvider.GetForwardedHeaders())
            {
                source.Add(httpHeader.Key, httpHeader.Value);
            }

            return source;
        };

    }
}
}