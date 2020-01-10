using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Deployment.AutoUpload.ViewModels.AddCommand;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;
using Tauron.Application.Deployment.AutoUpload.ViewModels.VersionRepoManagerCommand;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.UploadCommand
{
    public sealed class UploadCommandContext : OperationContextBase, IContextApply
    {
        public RegistratedRepository? Repository { get; set; }

        public VersionRepository? VersionRepository { get; set; }

        public void Apply(OperationContextBase context)
        {
            switch (context)
            {
                case AddCommandContext addContext:
                    Repository = addContext.RegistratedRepository;
                    break;
                case VersionRepoContext versionRepoContext:
                    VersionRepository = versionRepoContext.VersionRepository;
                    break;
            }
        }
    }
}