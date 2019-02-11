using System;
using JetBrains.Annotations;
using Tauron.Application.ProjectManager.Services.Data.Entitys;

namespace Tauron.Application.ProjectManager.Services.DTO
{
    [Serializable, PublicAPI]
    public sealed class JobRunDto
    {
        public JobRunDto(string jobName, long iterations, long amount, int length, int width, double speed, TimeSpan effectiveTime, bool isValid)
        {
            JobName = jobName;
            Iterations    = iterations;
            Amount        = amount;
            Length        = length;
            Width         = width;
            Speed         = speed;
            EffectiveTime = effectiveTime;
            IsValid       = isValid;
        }

        public string JobName { get; }

        public long Iterations { get; }

        public long Amount { get; }

        public int Length { get; }

        public int Width { get; }

        public double Speed { get; }

        public TimeSpan EffectiveTime { get; }

        public bool IsValid { get; }

        public static JobRunDto FromEntity(JobRunEntity entity)
        {
            return new JobRunDto(entity.JobId, entity.Iterations, entity.Amount, entity.Length, entity.Width, entity.Speed, entity.EffectiveTime, true);
        }
    }
}