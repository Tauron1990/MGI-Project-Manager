using Octokit;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.AddCommand
{
    public sealed class AddCommandContext : OperationContextBase
    {
        public Repository Repository { get; set; } = new Repository();

        public Branch Branch { get; set; } = new Branch();

        public string RealPath { get; set; } = string.Empty;
    }
}