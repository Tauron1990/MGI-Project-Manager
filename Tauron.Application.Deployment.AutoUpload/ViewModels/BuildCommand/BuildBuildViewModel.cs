using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Catel.Services;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Models.Build;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand
{
    [ServiceDescriptor(typeof(BuildBuildViewModel))]
    public class BuildBuildViewModel : OperationViewModel<BuildOperationContext>
    {
        private readonly IMessageService _messageService;
        private readonly Dispatcher _dispatcher;

        public int ErrorCount { get; set; }

        public string Console { get; set; } = string.Empty;

        public BuildBuildViewModel(IMessageService messageService, Dispatcher dispatcher)
        {
            _messageService = messageService;
            _dispatcher = dispatcher;
        }

        protected override async Task InitializeAsync()
        {
            if (BuildContext.CanBuild)
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
            var buildContext = Context.BuildContext;
            buildContext.Output += BuildContextOnOutput;
            buildContext.Error += BuildContextOnError;

            try
            {
                var targetPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? string.Empty, "Output");
                if (Directory.Exists(targetPath))
                    Directory.Delete(targetPath, true);
                Directory.CreateDirectory(targetPath);

                var result = await buildContext.TryBuild(Context.RegistratedRepository, targetPath);

                if (result != 0 && ErrorCount != 0)
                {
                    Context.Failed = new BuildFailed(ErrorCount, result, Console);
                    await OnNextView<BuildErrorViewModel>();
                }
                else if (Context.NoLocatonOpening)
                    await OnFinish("Erstellen erfolgreich");
                else
                    await OnNextView<BuildOpenLocationViewModel>();

                //TODO NextView
            }
            catch (Exception e)
            {
                await _messageService.ShowErrorAsync(e);
                await OnReturn();
            }
            finally
            {
                buildContext.Output -= BuildContextOnOutput;
                buildContext.Error -= BuildContextOnError;
            }
        }

        private void BuildContextOnError() => ErrorCount++;

        private void BuildContextOnOutput(string obj) => _dispatcher.Invoke(() => Console = Console + Environment.NewLine + obj, DispatcherPriority.Render);

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