using System.Threading.Tasks;

namespace Tauron.CQRS.Common.ServerHubs
{
    public interface IEventBus
    {
        Task Subscribe(string eventName, string apiKey);

        Task UnSubscribe(string eventName, string apiKey);
    }
}