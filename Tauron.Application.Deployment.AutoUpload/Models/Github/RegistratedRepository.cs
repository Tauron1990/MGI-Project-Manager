using System.IO;

namespace Tauron.Application.Deployment.AutoUpload.Models.Github
{
    public class RegistratedRepository
    {
        public long Id { get; }

        public string RepositoryName { get; }

        public string BranchName { get; }

        public string ProjectName { get; }

        public string RealPath { get; }

        public RegistratedRepository(long id, string branchName, string projectName, string repositoryName, string realPath)
        {
            Id = id;
            BranchName = branchName;
            ProjectName = projectName;
            RepositoryName = repositoryName;
            RealPath = realPath;
        }

        public override string ToString() => $"{RepositoryName} -- {Path.GetFileName(ProjectName)}";
    }
}