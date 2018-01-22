using System;

namespace Tauron.Application.MgiProjectManager.ServiceLayer.Dto
{
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