using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Tauron.Application.Deployment.AutoUpload.Models.Build
{
    [ServiceDescriptor(typeof(IBuildServer), ServiceLifetime.Scoped)]
    public sealed class BuildServer : IBuildServer
    {
        private readonly BuildDispatcher _dispatcher;

        public BuildServer(BuildDispatcher dispatcher) 
            => _dispatcher = dispatcher;

        public void String(string msg) 
            => _dispatcher.Send(msg);
    }
}