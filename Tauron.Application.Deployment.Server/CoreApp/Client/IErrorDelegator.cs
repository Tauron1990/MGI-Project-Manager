using System;

namespace Tauron.Application.Deployment.Server.CoreApp.Client
{
    public interface IErrorDelegator
    {
        public event Action<string>? ErrorRecived;

        public void PublishError(Exception error);
    }
}
