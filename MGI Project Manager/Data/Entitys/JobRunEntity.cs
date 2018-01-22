using System;
using Tauron.Application.Common.BaseLayer.Data;

namespace Tauron.Application.MgiProjectManager.Data.Entitys
{
    public sealed class JobRunEntity : GenericBaseEntity<int>
    {
        public string JobId { get; set; }

        public JobEntity Job { get; set; }

        public bool Problem { get; set; }

        public bool BigProblem { get; set; }

        public long Iterations { get; set; }

        public long Amount { get; set; }

        public int Length { get; set; }

        public int Width { get; set; }

        public double Speed { get; set; }

        public DateTime StartTime { get; set; }

        public TimeSpan NormaizedTime { get; set; }

        public TimeSpan EffectiveTime { get; set; }

        public int? SetupTime { get; set; }

        public int? IterationTime { get; set; }

        public bool IsValid { get; set; }
    }
}