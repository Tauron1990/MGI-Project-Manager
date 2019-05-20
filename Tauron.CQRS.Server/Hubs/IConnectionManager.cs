using System;
using System.Threading.Tasks;

namespace Tauron.CQRS.Server.Hubs
{
    public interface IConnectionManager
    {
        event Action<string> ClientAdded;

        event Action<string> ClientRemoved; 

        int GetCurrentClients(string eventName);

        Task AddToGroup(string connectionId, string group);

        Task AddConnection(string connectionId);

        Task RemoveConnection(string connectionId);

        Task RemoveFromGroup(string connectionId, string group);

        Task StillConnected(string connectionId);

        Task UpdateAllConnection();
    }
}