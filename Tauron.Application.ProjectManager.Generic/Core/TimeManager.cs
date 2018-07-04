using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using Tauron.Application.ProjectManager.Generic.Clients;

namespace Tauron.Application.ProjectManager.Generic.Core
{
    public class TimeManager : IDisposable
    {
        private static readonly List<TimeSpan> Reconnecttimes = CreateQueue();

        private static List<TimeSpan> CreateQueue()
        {
            List<TimeSpan> times = new List<TimeSpan>();

            for (int i = 0; i < 5; i++)
                Reconnecttimes.Add(TimeSpan.FromSeconds(5));
            for (int i = 0; i < 10; i++)
                Reconnecttimes.Add(TimeSpan.FromSeconds(10));
            for (int i = 0; i < 10; i++)
                Reconnecttimes.Add(TimeSpan.FromSeconds(30));
            Reconnecttimes.Add(TimeSpan.FromMinutes(5));

            return times;
        }

        private readonly Timer _sendTimer;
        private readonly Timer _timeoutTimer;
        private readonly Timer _reconnectTimer;
        private Queue<TimeSpan> _timeSpans;

        private void TryReConnect(object state) => StartListening();

        private void PingTimeOut(object state)
        {
            _client.Close();
            _client.OnConnectionLost();
            StartReconnect();
        }

        private void PingServer(object state)
        {
            try
            {
                _client.Ping();
                _timeoutTimer.Change(TimeSpan.FromMinutes(2), Timeout.InfiniteTimeSpan);
            }
            catch (Exception e) when(e is CommunicationException || e is NullReferenceException)
            {
                try
                {
                    _client.Close();
                }
                catch (CommunicationException)
                {
                }

                _client.OnConnectionLost();
                StartReconnect();
            }
        }

        private readonly JobPushMessageClient _client;

        public TimeManager(JobPushMessageClient client)
        {
            _client = client;

            _sendTimer      = new Timer(PingServer);
            _timeoutTimer   = new Timer(PingTimeOut);
            _reconnectTimer = new Timer(TryReConnect);
        }

        public void ConnectionAlive()
        {
            _timeoutTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _sendTimer.Change(TimeSpan.FromMinutes(5), Timeout.InfiniteTimeSpan);
        }

        public void Dispose()
        {
            _sendTimer?.Dispose();
            _timeoutTimer?.Dispose();
            _reconnectTimer?.Dispose();
        }

        public void StartListening()
        {
            try
            {
                _client.Open();
                _client.Register();
                _client.OnConnectionEstablisht();
                _timeSpans = null;
            }
            catch (ClientException)
            {
                _client.Abort();
                _client.OnConnectionLost();
                StartReconnect();
                _timeoutTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
                _sendTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            }
        }

        public void StopListening()
        {
            if(_client.State != CommunicationState.Opened) return;

            try
            {
                _client.Close();
            }
            catch(Exception e)
            {
                if (CriticalExceptions.IsCriticalApplicationException(e)) throw;
            }
        }

        private void StartReconnect()
        {
            _timeoutTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _sendTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

            if(_timeSpans == null)
                _timeSpans = new Queue<TimeSpan>(Reconnecttimes);

            var time = _timeSpans.Count == 1 ? _timeSpans.Peek() : _timeSpans.Dequeue();

            _reconnectTimer.Change(time, Timeout.InfiniteTimeSpan);
        }
    }
}