using System;

namespace Tauron.Application.MgiProjectManager.ServiceLayer.Dto
{
    public sealed class CalculateTimeOutput
    {
        public CalculateTimeOutput(TimeSpan? setupTime, TimeSpan? iterationTime, TimeSpan? runTime, string error, PrecisionMode precisionMode)
        {
            SetupTime     = setupTime;
            IterationTime = iterationTime;
            RunTime       = runTime;
            Error         = error;
            PrecisionMode = precisionMode;
        }

        public TimeSpan? SetupTime     { get; }
        public TimeSpan? IterationTime { get; }
        public TimeSpan? RunTime       { get; }

        public string        Error         { get; }
        public PrecisionMode PrecisionMode { get; }
    }
}