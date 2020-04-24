using Grpc.Core;
using JetBrains.Annotations;
using Scrutor;
using Tauron.Application.ToolUi.SimpleAuth;
using static Tauron.Application.Deployment.Server.Services.DownloadService;

namespace Tauron.Application.Deployment.ServerUI.Services
{
    [ServiceDescriptor(typeof(IClientFabricator<DownloadServiceClient>)), UsedImplicitly]
    public sealed class DownloadServiceFabricator : IClientFabricator<DownloadServiceClient>
    {
        public DownloadServiceClient Construct(ChannelBase invoker) 
            => new DownloadServiceClient(invoker);
    }
}