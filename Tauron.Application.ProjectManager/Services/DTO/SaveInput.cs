using System;
using JetBrains.Annotations;

namespace Tauron.Application.ProjectManager.Services.DTO
{
    [Serializable, PublicAPI]
    public sealed class SaveInput
    {
        public SaveInput(long? amount, long?   iteratins, bool           problem, bool bigProblem, PaperFormat paperFormat, double? speed, DateTime startTime, TimeSpan runTime,
                         int?  setupTime, int? iterationTime, JobItemDto jobItem)
        {
            Amount        = amount;
            Iteratins     = iteratins;
            Problem       = problem;
            BigProblem    = bigProblem;
            PaperFormat   = paperFormat;
            Speed         = speed;
            StartTime     = startTime;
            RunTime       = runTime;
            SetupTime     = setupTime;
            IterationTime = iterationTime;
            JobItem       = jobItem;
        }
        
        public long?       Amount        { get; }
        public long?       Iteratins     { get; }
        public bool        Problem       { get; }
        public bool        BigProblem    { get; }
        public PaperFormat PaperFormat   { get; }
        public double?     Speed         { get; }
        public DateTime    StartTime     { get; }
        public TimeSpan    RunTime       { get; }
        public int?        SetupTime     { get; }
        public int?        IterationTime { get; }
        public JobItemDto  JobItem       { get; }
    }
}