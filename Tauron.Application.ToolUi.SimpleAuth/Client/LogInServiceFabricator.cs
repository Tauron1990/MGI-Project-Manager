using Grpc.Core;
using JetBrains.Annotations;
using Scrutor;
using Tauron.Application.SimpleAuth.Api.Proto;

namespace Tauron.Application.ToolUi.SimpleAuth.Client
{
    [UsedImplicitly, ServiceDescriptor(typeof(IClientFabricator<LoginService.LoginServiceClient>))]
    public sealed class LogInServiceFabricator : IClientFabricator<LoginService.LoginServiceClient>
    {
        public LoginService.LoginServiceClient Construct(CallInvoker invoker) 
            => new LoginService.LoginServiceClient(invoker);
    }
}