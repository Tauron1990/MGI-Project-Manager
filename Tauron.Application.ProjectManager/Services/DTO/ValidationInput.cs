using System;
using JetBrains.Annotations;

namespace Tauron.Application.ProjectManager.Services.DTO
{
    [Serializable, PublicAPI]
    public sealed class ValidationInput
    {
        public ValidationInput(string name, long? amount, long? iteration, TimeSpan runTime, PaperFormat format, double? speed)
        {
            Name = name;
            Amount    = amount;
            Iteration = iteration;
            RunTime   = runTime;
            Format    = format;
            Speed     = speed;
        }

        public string Name { get; }
        public long?       Amount    { get; }
        public long?       Iteration { get; }
        public TimeSpan    RunTime   { get; }
        public PaperFormat Format    { get; }
        public double?     Speed     { get; }
    }
}