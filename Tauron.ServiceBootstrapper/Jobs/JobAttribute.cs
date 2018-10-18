using System;
using JetBrains.Annotations;

namespace Tauron.ServiceBootstrapper.Jobs
{
    [AttributeUsage(AttributeTargets.Class)]
    [BaseTypeRequired(typeof(IJob))]
    public class JobAttribute : Attribute
    {
        private readonly double _hourers;

        public JobAttribute(double hourers) 
            => _hourers = hourers;

        public TimeSpan ToInterval() 
            => TimeSpan.FromHours(_hourers);
    }
}