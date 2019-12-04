using System;
using System.Threading.Tasks;
using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Common;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand
{
    public sealed class PotentialProjekt : INameable
    {
        public RegistratedRepository Repository { get; }

        public string Name => Repository.ProjectName;

        public PotentialProjekt(RegistratedRepository repository) 
            => Repository = repository;

    }
}