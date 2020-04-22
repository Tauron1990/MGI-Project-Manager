using Grpc.Core;

namespace Tauron.Application.ToolUi.SimpleAuth
{
    public interface IClientFabricator<out TClient>
        where TClient : ClientBase
    {
        TClient Construct(ChannelBase invoker);
    }
}