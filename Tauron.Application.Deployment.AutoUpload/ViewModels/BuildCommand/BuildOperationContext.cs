using Tauron.Application.Deployment.AutoUpload.Models.Build;
using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Deployment.AutoUpload.ViewModels.AddCommand;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand
{
    public class BuildOperationContext : OperationContextBase
    {
        private RegistratedRepository? _registratedRepository;

        public BuildContext BuildContext { get; }

        public RegistratedRepository? RegistratedRepository
        {
            get => _registratedRepository ?? (Redirection?.ParentContext as AddCommandContext)?.RegistratedRepository;
            set => _registratedRepository = value;
        }

        public bool NoLocatonOpening { get; set; }

        public BuildOperationContext(BuildContext buildContext) => BuildContext = buildContext;
    }
}