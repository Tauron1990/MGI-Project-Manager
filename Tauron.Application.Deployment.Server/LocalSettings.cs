using Tauron.Application.Deployment.Server.Engine;
using Tauron.Application.Deployment.Server.Engine.Provider;

namespace Tauron.Application.Deployment.Server
{
    public sealed class LocalSettings
    {
        public ServerFileMode ServerFileMode { get; set; }

        public string DatabaseName { get; set; } = string.Empty;

        public GitSignature Signature { get; set; } = new GitSignature();
    }
}