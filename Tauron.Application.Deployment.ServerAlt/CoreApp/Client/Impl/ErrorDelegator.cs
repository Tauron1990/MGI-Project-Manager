using System;

namespace Tauron.Application.Deployment.Server.CoreApp.Client.Impl
{
    public sealed class ErrorDelegator : IErrorDelegator
    {
        public event Action<string>? ErrorRecived;

        public void PublishError(Exception error)
            => ErrorRecived?.Invoke($"{error.GetType().Name} -- {error.Message}");

        public void PublishError(string msg)
            => ErrorRecived?.Invoke(msg);
    }
}