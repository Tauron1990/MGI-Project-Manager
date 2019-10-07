using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

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
        private readonly ILogger<IConnectionManager> _logger;

        public event Action<string> ClientAdded;
        public event Action<string> ClientRemoved;

        public ConnectionManager(ILogger<IConnectionManager> logger)
        {
            _logger = logger;
        }

        public int GetCurrentClients(string eventName) => _connections.Where(c => c.Value.Contains(eventName)).Sum(c => 1);

        public Task AddToGroup(string connectionId, string group)
        {
            _logger.LogInformation($"AddToGroup {connectionId} -- {group}");

            if (!_connections.TryGetValue(connectionId, out var bag)) return Task.CompletedTask;

            lock (bag)
                bag.Add(group);

            OnClientAdded(group);

            return Task.CompletedTask;
        }

        public Task AddConnection(string connectionId)
        {
            _logger.LogInformation($"AddConnection {connectionId}");

            _connections.TryAdd(connectionId, new Connection());
            return Task.CompletedTask;
        }

        public Task RemoveConnection(string connectionId)
        {
            _logger.LogInformation($"RemoveConnection {connectionId}");

            _connections.TryRemove(connectionId, out _);
            return Task.CompletedTask;
        }

        public Task RemoveFromGroup(string connectionId, string group)
        {
            _logger.LogInformation($"RemoveFromGroup {connectionId} -- {group}");

            if (!_connections.TryGetValue(connectionId, out var bag)) return Task.CompletedTask;

            lock (bag)
                bag.Remove(group);

            OnClientRemoved(group);

            return Task.CompletedTask;
        }

        public Task StillConnected(string connectionId)
        {
            _logger.LogInformation($"StillConnected {connectionId}");

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
            //_logger.LogInformation("UpdateAllConnection");

            foreach (var connection in _connections)
            {
                lock (connection.Value)
                {
                    if (connection.Value.Stopwatch.ElapsedMilliseconds <= 60000) continue;

                    _logger.LogWarning($"Unstable Connection: {connection.Key}");
                    connection.Value.Stade = ConnectionStade.Unstable;
                    foreach (var eventType in connection.Value)
                        OnClientRemoved(eventType);
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