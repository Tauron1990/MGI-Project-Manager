using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.VersionRepoManagerCommand
{
    public sealed class VersionRepoContext : OperationContextBase
    {
        public VersionRepository? VersionRepository { get; set; }
    }
}