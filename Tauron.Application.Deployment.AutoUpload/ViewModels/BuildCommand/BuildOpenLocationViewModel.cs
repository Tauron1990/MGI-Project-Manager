using System.Threading.Tasks;
using System.Windows.Documents;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand
{
    [ServiceDescriptor(typeof(BuildOpenLocationViewModel))]
    public sealed class BuildOpenLocationViewModel : OperationViewModel<BuildOperationContext>
    {
        public string Location { get; set; } = string.Empty;

        protected override async Task InitializeAsync()
        {

            await base.InitializeAsync();
        }
    }
}