using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tauron.ServiceBootstrapper.Jobs;

namespace Tauron.ServiceBootstrapper.Core
{
    public sealed class JobManager : IJobManager
    {
        private class JobOperator
        {
            private readonly Type _jobType;
            private readonly IServiceScopeFactory _scopeFactory;
            private readonly TimeSpan _interval;
            private readonly ILogger _logger;
            private readonly JobContext _context;

            private DateTime _lastRun = DateTime.Now;

            public JobOperator(Type jobType, IServiceScopeFactory scopeFactory, TimeSpan interval, ILogger logger, JobContext context)
            {
                _jobType = jobType;
                _scopeFactory = scopeFactory;
                _interval = interval;
                _logger = logger;
                _context = context;
            }

            public async Task TryRun()
            {
                try
                {
                    var dateNow = DateTime.Now;
                    if(_lastRun + _interval > dateNow)
                        return;

                    using var scope = _scopeFactory.CreateScope();
                    var job = (IJob) ActivatorUtilities.CreateInstance(scope.ServiceProvider, _jobType);

                    await job.Invoke(_context);

                    _lastRun = dateNow;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error on Run Job {_jobType.Name}");
                }
            }
        }

        private readonly List<JobOperator> _jobOperators = new List<JobOperator>();
        private readonly ILogger<JobManager> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly Timer _timer;

        private int _stop;
        private JobContext _jobContext;

        public JobManager(ILogger<JobManager> logger, IServiceScopeFactory scopeFactory)
        {
            _timer = new Timer(Interval);
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        private async void Interval(object state)
        {
            foreach (var jobOperator in _jobOperators) 
                await jobOperator.TryRun();

            _timer.Change(TimeSpan.FromMinutes(2), Timeout.InfiniteTimeSpan);
        }

        public void Start()
        {
            _jobContext = new JobContext();

            _jobOperators.Clear();
            _jobOperators.AddRange(JobStore.GetJobs().Select(fj => new JobOperator(fj.JobType, _scopeFactory, fj.Interval, _logger, _jobContext)));

            _timer.Change(TimeSpan.FromSeconds(10), Timeout.InfiniteTimeSpan);
        }

        public void Stop() => Interlocked.Exchange(ref _stop, 1);

        public void Dispose() 
            => _timer.Dispose();
    }
}