using System.IO;
using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Common;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand
{
    public sealed class PotentialProjekt : INameable
    {
        public PotentialProjekt(RegistratedRepository repository)
        {
            Repository = repository;
        }

        public RegistratedRepository Repository { get; }

        public string Name => Path.GetFileName(Repository.ProjectName);
    }
}