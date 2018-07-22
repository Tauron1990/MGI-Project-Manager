using System;
using System.Collections.Concurrent;
using System.Linq;
using System.ServiceModel;
using Tauron.Application.ProjectManager.Resources;
using Tauron.Application.ProjectManager.Services;

namespace Tauron.Application.ProjectManager.ApplicationServer.Core
{
    public static class ConnectivityManager
    {
        private class WeakConcurrentDicionary<TKey, TValue> : ConcurrentDictionary<TKey, TValue>
            where TKey : IWeakReference
        {
            public WeakConcurrentDicionary() => WeakCleanUp.RegisterAction(CleanUp);

            private void CleanUp()
            {
                foreach (var key in Keys.Where(key => !key.IsAlive).ToArray())
                    TryRemove(key, out _);
            }
        }

        private class SessionKey : IWeakReference, IEquatable<SessionKey>
        {
            private readonly IContextChannel _channel;
            private string SessionId { get; }

            public bool IsAlive => _channel?.State == CommunicationState.Opened;

            public SessionKey(IContextChannel channel, string sessionId)
            {
                _channel = channel;
                SessionId = sessionId;
            }

            private SessionKey(string id) => SessionId = id;

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == GetType() && Equals((SessionKey) obj);
            }

            public bool Equals(SessionKey other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(_channel, other._channel) && string.Equals(SessionId, other.SessionId);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((_channel != null ? _channel.GetHashCode() : 0) * 397) ^ (SessionId != null ? SessionId.GetHashCode() : 0);
                }
            }

            public static bool operator ==(SessionKey left, SessionKey right) => Equals(left, right);

            public static bool operator !=(SessionKey left, SessionKey right) => !Equals(left, right);

            public static implicit operator SessionKey(string id) => new SessionKey(id);

            public static implicit operator string(SessionKey key) => key.SessionId;
        }

        private static readonly WeakConcurrentDicionary<SessionKey, IJobPushMessageCallback> Callbacks = new WeakConcurrentDicionary<SessionKey, IJobPushMessageCallback>();

        public static void Close()
        {
            foreach (var messageCallback in from callback in Callbacks
                                            where callback.Key.IsAlive
                                            select callback.Value)
                try
                {
                    messageCallback.Close();
                }
                catch (CommunicationException)
                {

                }

            Callbacks.Clear();
        }

        public static void SendPong(string id)
        {
            try
            {
                if (Callbacks.TryGetValue(id, out var messageCallback))
                    messageCallback.Pong();
            }
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            catch (Exception e) when(e is CommunicationException && e is ObjectDisposedException)
            {
            }
        }

        public static void Register(string sessionId, IContextChannel channel, IJobPushMessageCallback callback)
        {
            if (!Callbacks.TryAdd(new SessionKey(channel, sessionId), callback))
                throw new FaultException(new FaultReason(ServiceErrorMessages.ConnectivityManager_Session_AlreadyRegistered));
        }

        public static void UnResgister(string sessionId)
        {
            Callbacks.TryRemove(sessionId, out _);
        }

        public static void Inform(Action<IJobPushMessageCallback> action)
        {
            foreach (var callback in Callbacks.Where(c => c.Key.IsAlive).Select(c => c.Value))
            {
                try
                {
                    action(callback);
                }
                catch (CommunicationException)
                {
                }
            }
        }
    }
}