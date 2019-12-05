using System;
using Tauron.Application.Deployment.AutoUpload.Models.Build;
using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Deployment.AutoUpload.ViewModels.AddCommand;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand
{
    public class BuildFailed
    {
        public int ErrorCount { get; }

        public int Result { get; }

        public string Console { get; }

        public BuildFailed(int errorCount, int result, string console)
        {
            ErrorCount = errorCount;
            Result = result;
            Console = console;
        }
    }

    public class BuildOperationContext : OperationContextBase
    {
        private RegistratedRepository? _registratedRepository;

        public BuildContext BuildContext { get; }

        public RegistratedRepository? RegistratedRepository
        {
            get => _registratedRepository ?? (Redirection?.ParentContext as AddCommandContext)?.RegistratedRepository;
            set => _registratedRepository = value;
        }

        public BuildFailed? Failed { get; set; }

        public bool NoLocatonOpening { get; set; }

        public BuildOperationContext(BuildContext buildContext) => BuildContext = buildContext;
    }
}