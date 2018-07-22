using System;
using JetBrains.Annotations;

namespace Tauron.Application.ProjectManager.Services.DTO
{
    [Serializable, PublicAPI]
    public class SaveOutput
    {
        public SaveOutput(bool succsess, string message)
        {
            Succsess = succsess;
            Message  = message;
        }

        public bool   Succsess { get; }
        public string Message  { get; }
    }
}