using System.Diagnostics;
using System.Threading.Tasks;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand
{
    [ServiceDescriptor(typeof(BuildOpenLocationViewModel))]
    public sealed class BuildOpenLocationViewModel : OperationViewModel<BuildOperationContext>
    {
        public string Location { get; set; } = string.Empty;

        protected override async Task InitializeAsync()
        {
            Location = Context.Location;

            await base.InitializeAsync();
        }

        [CommandTarget]
        public async Task OnNext()
        {
            await OnFinish();
        }

        [CommandTarget]
        public void OnOpen()
        {
            Process.Start("explorer.exe", Location);
        }
    }
}