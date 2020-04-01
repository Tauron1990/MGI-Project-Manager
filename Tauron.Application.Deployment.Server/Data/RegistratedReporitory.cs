using Tauron.Application.Deployment.Server.Engine.Data;

namespace Tauron.Application.Deployment.Server.Data
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

        public RegistratedReporitory(RegistratedReporitoryEntity reporitory, RepositoryProvider provider)
        {
            Name = reporitory.Name;
            Comment = reporitory.Comment;
            Provider = provider;
        }
    }
}