namespace Tauron.Application.Deployment.Server.Engine.Data
{
    public sealed class RegistratedRepositoryEntity
    {
        public string? Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Source { get; set; } = string.Empty;

        public string TargetPath { get; set; } = string.Empty;

        public string Comment { get; set; } = string.Empty;

        public string Provider { get; set; } = string.Empty;

        public bool SyncCompled { get; set; }
    }
}