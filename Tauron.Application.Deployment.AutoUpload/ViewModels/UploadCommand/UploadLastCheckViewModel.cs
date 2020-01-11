using System.Threading.Tasks;
using Catel.IoC;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Models.Build;
using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.UploadCommand
{
    [ServiceDescriptor(typeof(UploadLastCheckViewModel))]
    public class UploadLastCheckViewModel : OperationViewModel<UploadCommandContext>
    {
        public RegistratedRepository? Repository { get; set; }

        public VersionRepository? VersionRepository { get; set; }

        public bool? VersionOk { get; set; } = false;

        public bool? RepoOk { get; set; } = false;

        protected override Task InitializeAsync()
        {
            Repository = Context.Repository;
            VersionRepository = Context.VersionRepository;

            return base.InitializeAsync();
        }

        [CommandTarget]
        public async Task OnNext()
        {
            if(VersionOk != true || RepoOk != true) return;

            await OnNextView<BuildBuildViewModel, BuildOperationContext>(new BuildOperationContext(DependencyResolver.Resolve<BuildContext>()), CreateRedirection<UploadCreatePackageViewModel>());
        }

        [CommandTarget]
        public bool CanOnNext() => VersionOk == true && RepoOk == true;

    }
}