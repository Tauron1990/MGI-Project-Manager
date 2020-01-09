using Scrutor;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Common;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.UploadCommand
{
    [ServiceDescriptor(typeof(UploadSelectRepoViewModel))]
    public class UploadSelectRepoViewModel : OperationViewModel<UploadCommandContext>
    {
        public ICommonSelectorViewModel RepoSelector { get; set; }
    }
}