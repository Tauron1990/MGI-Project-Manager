using System;

namespace Tauron.Application.Deployment.AutoUpload.Core
{
    public sealed class CommonError : Exception
    {
        public CommonError(string message)
            : base(message)
        {
            
        }
    }
}