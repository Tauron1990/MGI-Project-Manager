using System.Threading.Tasks;
using Tauron.MgiProjectManager.Dispatcher.Model;

namespace Tauron.MgiProjectManager.Dispatcher
{
    public interface IEventDispatcher
    {
        TypedEventToken<TEvent> GetToken<TEvent>();
    }
}