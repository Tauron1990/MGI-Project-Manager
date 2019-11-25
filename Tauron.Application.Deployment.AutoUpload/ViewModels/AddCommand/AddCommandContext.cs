using Octokit;
using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.AddCommand
{
    public sealed class AddCommandContext : OperationContextBase
    {
        public RegistratedRepository? RegistratedRepository { get; private set; }

        public Repository Repository { get; set; } = new Repository();

        public Branch Branch { get; set; } = new Branch();

        public string RealPath { get; set; } = string.Empty;

        public RegistratedRepository CreateRegistratedRepository(string fileName)
        {
            RegistratedRepository = new RegistratedRepository(Repository.Id, Branch.Name, fileName, Repository.FullName, RealPath);
            return RegistratedRepository;
        }
    }
}