using System.Threading.Tasks;
using Catel.Services;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand
{
    [ServiceDescriptor(typeof(BuildBuildViewModel))]
    public class BuildBuildViewModel : OperationViewModel<BuildOperationContext>
    {
        private readonly IMessageService _messageService;

        public BuildBuildViewModel(IMessageService messageService) 
            => _messageService = messageService;

        protected override async Task InitializeAsync()
        {
            if (Context.BuildContext.CanBuild)
            {
                if(await _messageService.ShowAsync("Dot Net Core Framework nicht gefunden. Webseite öffnen?", "Fehler", MessageButton.YesNo, MessageImage.Error) == MessageResult.Yes)

                const string website = "https://dotnet.microsoft.com/download";
                UI.WriteLine("");
                if (await UI.Allow(website + " Öffnen:"))
                    OpenWebsite(website);
            }
        }
    }
}