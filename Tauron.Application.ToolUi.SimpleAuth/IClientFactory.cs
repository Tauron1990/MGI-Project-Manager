using System.Threading.Tasks;
using Grpc.Core;

namespace Tauron.Application.ToolUi.SimpleAuth
{
    public interface IClientFactory<out TClient>
        where TClient : ClientBase
    {
        TClient Create(string host);
    }
}