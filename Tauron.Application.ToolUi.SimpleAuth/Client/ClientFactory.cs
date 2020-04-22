using Grpc.Core;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Tauron.Application.ToolUi.SimpleAuth.Client
{
    [ServiceDescriptor(typeof(IClientFactory<>), ServiceLifetime.Transient)]
    [UsedImplicitly]
    public sealed class ClientFactory<TClient> : IClientFactory<TClient>
        where TClient : ClientBase
    {
        private readonly IClientFabricator<TClient> _fabricator;
        private readonly ChannelManager _channelManager;

        public ClientFactory(IClientFabricator<TClient> fabricator, ChannelManager channelManager)
        {
            _fabricator = fabricator;
            _channelManager = channelManager;
        }


        public TClient Create(string host) 
            => _fabricator.Construct(_channelManager.Channel(host));
    }
}