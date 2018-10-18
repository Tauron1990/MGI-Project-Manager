using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tauron.ServiceBootstrapper.Jobs;

namespace Tauron.ServiceBootstrapper.Core
{
    public static class JobStore
    {
        public class FoundJob
        {
            public TimeSpan Interval { get; }

            public Type JobType { get; }

            public FoundJob(TimeSpan interval, Type jobType)
            {
                Interval = interval;
                JobType = jobType;
            }
        } 

        private static readonly HashSet<FoundJob> FoundJobs = new HashSet<FoundJob>();

        public static void SearchAt<TType>()
        {
            foreach (var type in typeof(TType).Assembly.GetTypes())
            {
                if(!(type.GetCustomAttribute(typeof(JobAttribute)) is JobAttribute attr)) return;

                FoundJobs.Add(new FoundJob(attr.ToInterval(), type));
            }
        }

        internal static IEnumerable<FoundJob> GetJobs() => FoundJobs.ToList();
    }
}