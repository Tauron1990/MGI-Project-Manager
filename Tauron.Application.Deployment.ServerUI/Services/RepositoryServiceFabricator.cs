using Grpc.Core;
using JetBrains.Annotations;
using Scrutor;
using Tauron.Application.ToolUi.SimpleAuth;
using static Tauron.Application.Deployment.Server.Services.RepositoryService;

namespace Tauron.Application.Deployment.ServerUI.Services
{
    [ServiceDescriptor(typeof(IClientFabricator<RepositoryServiceClient>)), UsedImplicitly]
    public sealed class RepositoryServiceFabricator : IClientFabricator<RepositoryServiceClient>
    {
        public RepositoryServiceClient Construct(ChannelBase invoker) => new RepositoryServiceClient(invoker);
    }
}