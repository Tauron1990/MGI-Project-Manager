using System;

namespace Tauron.Application.ProjectManager.Generic
{
    public sealed class ClientException : Exception
    {
        public ClientException(string message, Exception source)
            : base(message, source)
        {
            
        }
    }
}