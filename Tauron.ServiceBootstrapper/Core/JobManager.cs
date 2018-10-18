using Microsoft.Extensions.Logging;
using Tauron.ServiceBootstrapper.Jobs;

namespace Tauron.ServiceBootstrapper.Core
{
    public sealed class JobManager : IJobManager
    {
        private class JobOperator
        {
            public JobOperator()
            {
                asf
            }
        }

        private readonly ILogger<JobManager> _logger;

        public JobManager(ILogger<JobManager> logger) => _logger = logger;

        public void Start()
        {
            throw new System.NotImplementedException();
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
        }
        
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}