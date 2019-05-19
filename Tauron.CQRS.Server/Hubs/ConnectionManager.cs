using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Tauron.CQRS.Server.Hubs
{
    [UsedImplicitly]
    public class ConnectionManager : IConnectionManager
    {
        private readonly ConcurrentDictionary<string, HashSet<string>> _connections = new ConcurrentDictionary<string, HashSet<string>>();

        public Task AddToGroup(string connectionId, string group)
        {
            if (!_connections.TryGetValue(connectionId, out var bag)) return Task.CompletedTask;

            lock (bag)
                bag.Add(group);

            return Task.CompletedTask;
        }

        public Task AddConnection(string connectionId)
        {
            _connections.TryAdd(connectionId, new HashSet<string>());
            return Task.CompletedTask;
        }

        public Task RemoveConnection(string connectionId)
        {
            _connections.TryRemove(connectionId, out _);
            return Task.CompletedTask;
        }

        public Task RemoveFromGroup(string connectionId, string group)
        {
            if (!_connections.TryGetValue(connectionId, out var bag)) return Task.CompletedTask;

            lock (bag)
                bag.Remove(group);

            return Task.CompletedTask;
        }
    }
}