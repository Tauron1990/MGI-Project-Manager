namespace Tauron.Application.Deployment.AutoUpload.Github
{
    public class RegistratedRepository
    {
        public long Id { get; }

        public string BranchName { get; }

        public string ProjectName { get; }

        public RegistratedRepository(long id, string branchName, string projectName)
        {
            Id = id;
            BranchName = branchName;
            ProjectName = projectName;
        }
    }
}