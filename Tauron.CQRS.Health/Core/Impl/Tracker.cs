using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Tauron.CQRS.Common.Health;

namespace Tauron.CQRS.Health.Core.Impl
{
    public class Tracker : IStatisticsTracker
    {
        private class CalcEntry
        {
            public int Requests { get; }

            public CalcEntry(int requests) 
                => Requests = requests;
        }

        private int _allRequests;
        private int _currentRequests;
        private Stopwatch _stopwatch = new Stopwatch();

        private readonly long _houerTime;
        private readonly Buffer<CalcEntry> _calcEntries = new Buffer<CalcEntry>(1000);

        public Tracker()
        {
            _stopwatch.Start();
            _houerTime = TimeSpan.FromHours(1).Milliseconds;
        }

        public HealthData GenerateData()
        {
            var all = (double)_calcEntries.Count;

            return new HealthData
            {
                AllRequests = _allRequests,
                RequestsPerHouer = (int) (_calcEntries.Sum(s => s.Requests) / all)
            };
        }

        public Task AddRequest(HttpContext context)
        {
            Interlocked.Increment(ref _allRequests);
            Interlocked.Increment(ref _currentRequests);

            if (_stopwatch.ElapsedMilliseconds <= _houerTime) return Task.CompletedTask;

            lock (this)
            {
                if (_stopwatch.ElapsedMilliseconds <= _houerTime) return Task.CompletedTask;

                _calcEntries.Add(new CalcEntry(_currentRequests));

                Stopwatch temp = new Stopwatch();
                temp.Start();

                Interlocked.Exchange(ref _stopwatch, temp);
                Interlocked.Exchange(ref _currentRequests, 0);
            }

            return Task.CompletedTask;
        }
    }
}