using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tauron.ServiceBootstrapper.Jobs
{
    public class JobContext
    {
        private readonly Stopwatch _stopwatch;

        public TimeSpan RunTime => _stopwatch.Elapsed;

        public Dictionary<string, object> Data { get; } = new Dictionary<string, object>();

        public JobContext() 
            => _stopwatch = Stopwatch.StartNew();
    }
}