using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Deployment.AutoUpload.ViewModels.AddCommand;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.UploadCommand
{
    public sealed class UploadCommandContext : OperationContextBase, IContextApply
    {
        public RegistratedRepository? Repository { get; set; }

        public void Apply(OperationContextBase context)
        {
            Repository = context switch
            {
                AddCommandContext addContext => addContext.RegistratedRepository,
                _ => Repository
            };
        }
    }
}