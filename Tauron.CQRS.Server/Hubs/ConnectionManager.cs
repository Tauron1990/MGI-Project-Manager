using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Tauron.CQRS.Server.Hubs
{
    [UsedImplicitly]
    public sealed class ConnectionManager : IConnectionManager
    {
        private enum ConnectionStade
        {
            Stable,
            Unstable
        }

        private class Connection : HashSet<string>
        {
            public ConnectionStade Stade { get; set; } = ConnectionStade.Stable;

            public Stopwatch Stopwatch { get; } = new Stopwatch(); 
        }

        private readonly ConcurrentDictionary<string, Connection> _connections = new ConcurrentDictionary<string, Connection>();

        public event Action<string> ClientAdded;
        public event Action<string> ClientRemoved;

        public int GetCurrentClients(string eventName)
        {
            if (!_connections.TryGetValue(eventName, out var connection)) return 0;

            lock (connection)
                return connection.Count;
        }

        public Task AddToGroup(string connectionId, string group)
        {
            if (!_connections.TryGetValue(connectionId, out var bag)) return Task.CompletedTask;

            lock (bag)
                bag.Add(group);

            OnClientAdded(group);

            return Task.CompletedTask;
        }

        public Task AddConnection(string connectionId)
        {
            _connections.TryAdd(connectionId, new Connection());
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

            OnClientRemoved(group);

            return Task.CompletedTask;
        }

        public Task StillConnected(string connectionId)
        {
            if (!_connections.TryGetValue(connectionId, out var bag)) return Task.CompletedTask;

            lock (bag)
            {
                if (bag.Stade == ConnectionStade.Unstable)
                {
                    foreach (var eventType in bag)
                        OnClientAdded(eventType);
                }

                bag.Stade = ConnectionStade.Stable;
                bag.Stopwatch.Restart();
            }

            return Task.CompletedTask;
        }

        public Task UpdateAllConnection()
        {
            foreach (var connection in _connections.Select(e => e.Value))
            {
                lock (connection)
                {
                    if (connection.Stopwatch.ElapsedMilliseconds > 60000)
                    {
                        connection.Stade = ConnectionStade.Unstable;
                        foreach (var eventType in connection)
                            OnClientRemoved(eventType);
                    }
                }
            }

            return Task.CompletedTask;
        }

        private void OnClientAdded(string obj) 
            => ClientAdded?.Invoke(obj);

        private void OnClientRemoved(string obj) 
            => ClientRemoved?.Invoke(obj);
    }
}