using System;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Tauron.Application.Deployment.AutoUpload.Models.Build
{
    [ServiceDescriptor(typeof(BuildDispatcher), ServiceLifetime.Scoped)]
    public sealed class BuildDispatcher
    {
        public event Action<string>? MessageRecived;

        public void Send(string msg)
            => MessageRecived?.Invoke(msg);
    }
}