using System;

namespace Tauron.Application.MgiProjectManager.ServiceLayer.Dto
{
    public sealed class ValidationInput
    {
        public ValidationInput(long? amount, long? iteration, TimeSpan runTime, PaperFormat format, double? speed)
        {
            Amount    = amount;
            Iteration = iteration;
            RunTime   = runTime;
            Format    = format;
            Speed     = speed;
        }

        public long?       Amount    { get; }
        public long?       Iteration { get; }
        public TimeSpan    RunTime   { get; }
        public PaperFormat Format    { get; }
        public double?     Speed     { get; }
    }
}