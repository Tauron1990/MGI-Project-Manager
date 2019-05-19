using System.Threading.Tasks;

namespace Tauron.CQRS.Server.Hubs
{
    public interface IConnectionManager
    {
        int Get //TODO Tracking For Connection and Event for EventCookie Countdown

        Task AddToGroup(string connectionId, string group);

        Task AddConnection(string connectionId);

        Task RemoveConnection(string connectionId);

        Task RemoveFromGroup(string connectionId, string group);
    }
}