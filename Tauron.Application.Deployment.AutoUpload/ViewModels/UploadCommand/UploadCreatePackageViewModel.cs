using Scrutor;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.UploadCommand
{
    [ServiceDescriptor(typeof(UploadCreatePackageViewModel))]
    public class UploadCreatePackageViewModel : OperationViewModel<UploadCommandContext>
    {
        private bool _cancel;

        public string Console { get; set; } = string.Empty;
    }
}