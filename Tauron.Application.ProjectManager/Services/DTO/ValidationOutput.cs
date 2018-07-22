using System;
using JetBrains.Annotations;

namespace Tauron.Application.ProjectManager.Services.DTO
{
    [Serializable, PublicAPI]
    public class ValidationOutput
    {
        public ValidationOutput(string formatedResult, TimeSpan? normalizedTime)
        {
            FormatedResult = formatedResult;
            NormalizedTime = normalizedTime;
        }

        public string    FormatedResult { get; }
        public TimeSpan? NormalizedTime { get; }
    }
}