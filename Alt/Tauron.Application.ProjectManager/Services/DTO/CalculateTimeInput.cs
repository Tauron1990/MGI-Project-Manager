using System;
using JetBrains.Annotations;

namespace Tauron.Application.ProjectManager.Services.DTO
{
    [Serializable, PublicAPI]
    public sealed class CalculateTimeInput
    {
        public CalculateTimeInput(string name, long? iterations, PaperFormat paperFormat, long? amount, double? speed)
        {
            Name = name;
            Iterations  = iterations;
            PaperFormat = paperFormat;
            Amount      = amount;
            Speed       = speed;
        }

        public string Name { get; }
        public long?       Iterations  { get; }
        public PaperFormat PaperFormat { get; }
        public long?       Amount      { get; }
        public double?     Speed       { get; }
    }
}