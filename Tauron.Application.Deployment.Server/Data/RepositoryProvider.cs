namespace Tauron.Application.Deployment.Server.Data
{
    /// <summary>
    /// Eine Mögliche Quelle für einen Provider
    /// </summary>
    public sealed class RepositoryProvider
    {
        /// <summary>
        /// Die id mit der der Server den provider identifizeirt.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Der Name des providers
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Die Beschreibung der prviders
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}