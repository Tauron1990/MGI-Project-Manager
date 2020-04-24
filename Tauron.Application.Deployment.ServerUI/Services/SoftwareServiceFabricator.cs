using Grpc.Core;
using JetBrains.Annotations;
using Scrutor;
using Tauron.Application.ToolUi.SimpleAuth;
using static Tauron.Application.Deployment.Server.Services.SoftwareService;

namespace Tauron.Application.Deployment.ServerUI.Services
{
    [ServiceDescriptor(typeof(IClientFabricator<SoftwareServiceClient>)), UsedImplicitly]
    public sealed class SoftwareServiceFabricator : IClientFabricator<SoftwareServiceClient>

    {
        public SoftwareServiceClient Construct(ChannelBase invoker) 
            => new SoftwareServiceClient(invoker);
    }
}