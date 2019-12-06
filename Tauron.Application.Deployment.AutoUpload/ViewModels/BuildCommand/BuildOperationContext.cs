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

    public class BuildOperationContext : OperationContextBase, IContextApply
    {
        public BuildContext BuildContext { get; }

        public RegistratedRepository? RegistratedRepository { get; set; }

        public BuildFailed? Failed { get; set; }

        public string Location { get; set; } = string.Empty;

        public bool NoLocatonOpening { get; set; }

        public BuildOperationContext(BuildContext buildContext)
        {
            BuildContext = buildContext;
        }

        public void Apply(OperationContextBase context)
        {
            switch (context)
            {
                case AddCommandContext add:
                    RegistratedRepository = add.RegistratedRepository;
                    break;
            }
        }
    }
}