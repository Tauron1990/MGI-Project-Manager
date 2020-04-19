namespace Tauron.Application.Deployment.Server.Engine.Data
{
    /// <summary>
    /// Ein Registriertes Software Repository
    /// </summary>
    public class RegistratedReporitory
    {
        /// <summary>
        /// Der Name des Repositorys
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Eine Beschreibung des Repositorys
        /// </summary>
        public string Comment { get; set; } = string.Empty;
        /// <summary>
        /// Der Provider des Repositorys
        /// </summary>
        public RepositoryProvider Provider { get; set; } = new RepositoryProvider();

        public RegistratedReporitory()
        {
            
        }

        public RegistratedReporitory(RegistratedRepositoryEntity repository, RepositoryProvider provider)
        {
            Name = repository.Name;
            Comment = repository.Comment;
            Provider = provider;
        }
    }
}