namespace Tauron.Application.Deployment.AutoUpload.Models.Github
{
    public class RegistratedRepository
    {
        public long Id { get; }

        public string RepositoryName { get; }

        public string BranchName { get; }

        public string ProjectName { get; }

        public RegistratedRepository(long id, string branchName, string projectName, string repositoryName)
        {
            Id = id;
            BranchName = branchName;
            ProjectName = projectName;
            RepositoryName = repositoryName;
        }

        public override string ToString() => $"{RepositoryName} -- {ProjectName}";
    }
}