using Tauron.Application.Deployment.Server.Engine;

namespace Tauron.Application.Deployment.Server
{
    public sealed class LocalSettings
    {
        public ServerFileMode ServerFileMode { get; set; }

        public string DatabaseName { get; set; }
    }
}