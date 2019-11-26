using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
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

                const string website = "https://dotnet.microsoft.com/download";
                if (await _messageService.ShowAsync("Dot Net Core Framework nicht gefunden. Installieren?", "Fehler", MessageButton.YesNo, MessageImage.Error) == MessageResult.Yes)
                    OpenWebsite(website);
                OnCancelCommandExecute();
                return;
            }

            RunBuild();
        }

        private async void RunBuild()
        {
            try
            {
                var targetPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? string.Empty, "Output");
                if(Directory.Exists(targetPath))
                    Directory.Delete(targetPath, true);
                Directory.CreateDirectory(targetPath);

            }
            catch (Exception e)
            {
                await _messageService.ShowErrorAsync(e);
            }
        }

        private static void OpenWebsite(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}