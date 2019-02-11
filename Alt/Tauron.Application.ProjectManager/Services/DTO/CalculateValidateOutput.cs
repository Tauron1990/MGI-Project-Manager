using System;
using JetBrains.Annotations;

namespace Tauron.Application.ProjectManager.Services.DTO
{
    [PublicAPI, Serializable]
    public sealed class CalculateValidateOutput
    {
        public CalculateValidateOutput(bool valid, string message)
        {
            Valid   = valid;
            Message = message;
        }

        public bool   Valid   { get; }
        public string Message { get; }
    }
}