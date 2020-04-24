using Grpc.Core;
using JetBrains.Annotations;
using Scrutor;
using Tauron.Application.ToolUi.SimpleAuth;
using static Tauron.Application.Deployment.Server.Services.ErrorPushSubscription;

namespace Tauron.Application.Deployment.ServerUI.Services
{
    [ServiceDescriptor(typeof(IClientFabricator<ErrorPushSubscriptionClient>)), UsedImplicitly]
    public sealed class ErrorPushSubscriptionFabricator : IClientFabricator<ErrorPushSubscriptionClient>
    {
        public ErrorPushSubscriptionClient Construct(ChannelBase invoker) 
            => new ErrorPushSubscriptionClient(invoker);
    }
}