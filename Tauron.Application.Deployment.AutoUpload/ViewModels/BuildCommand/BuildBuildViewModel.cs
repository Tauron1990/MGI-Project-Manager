using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Threading;
using Anotar.Serilog;
using Catel.Services;
using Scrutor;
using Serilog.Context;
using Tauron.Application.Deployment.AutoUpload.Models.Build;
using Tauron.Application.Deployment.AutoUpload.Models.Git;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;
using Tauron.Application.ToolUI.Core;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand
{
    [ServiceDescriptor(typeof(BuildBuildViewModel))]
    public class BuildBuildViewModel : OperationViewModel<BuildOperationContext>
    {
        private readonly Dispatcher _dispatcher;
        private readonly GitManager _gitManager;
        private readonly AppInfo _appInfo;
        private readonly IMessageService _messageService;

        public BuildBuildViewModel(IMessageService messageService, Dispatcher dispatcher, GitManager gitManager, AppInfo appInfo)
        {
            _messageService = messageService;
            _dispatcher = dispatcher;
            _gitManager = gitManager;
            _appInfo = appInfo;
        }

        public int ErrorCount { get; set; }

        public string Console { get; set; } = string.Empty;

        protected override async Task InitializeAsync()
        {
            if (!BuildContext.CanBuild)
            {
                LogTo.Warning("Net Core Framework not installed");
                const string website = "https://dotnet.microsoft.com/download";
                if (await _messageService.ShowAsync("Dot Net Core Framework nicht gefunden. Installieren?", "Fehler", MessageButton.YesNo, MessageImage.Error) == MessageResult.Yes)
                    await OpenWebsite(website);
                OnCancelCommandExecute();
                return;
            }

            RunBuild();
        }

        private async void RunBuild()
        {
            using (LogContext.PushProperty("Reposiory", Context.RegistratedRepository))
            {
                var buildContext = Context.BuildContext;
                buildContext.Output += BuildContextOnOutput;
                buildContext.Error += BuildContextOnError;

                try
                {
                    //var targetPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? string.Empty, "Output");
                    var targetPath = Path.Combine(_appInfo.SettingsDic, "Output");
                    Context.Location = targetPath;

                    if (Directory.Exists(targetPath))
                        Directory.Delete(targetPath, true);
                    Directory.CreateDirectory(targetPath);

                    LogTo.Information("Sync Repository");
                    _gitManager.SyncRepo(Context.RegistratedRepository?.RealPath ?? string.Empty);

                    LogTo.Information("Trying To Build Project");
                    var result = await buildContext.TryBuild(Context.RegistratedRepository, targetPath);

                    if (result != 0 && ErrorCount != 0)
                    {
                        LogTo.Information("Build Failed");
                        Context.Failed = new BuildFailed(ErrorCount, result, Console);
                        await OnNextView<BuildErrorViewModel>();
                    }
                    else
                    {
                        LogTo.Information("Build Compled");
                        if(Context.NoLocatonOpening)
                            await OnFinish("Erstellen erfolgreich");
                        else
                            await OnNextView<BuildOpenLocationViewModel>();
                    }
                }
                catch (Exception e)
                {
                    LogTo.Error(e, "Error On Build Project");
                    await _messageService.ShowErrorAsync(e);
                    await OnReturn();
                }
                finally
                {
                    buildContext.Output -= BuildContextOnOutput;
                    buildContext.Error -= BuildContextOnError;
                }
            }
        }

        private void BuildContextOnError()
        {
            ErrorCount++;
        }

        private void BuildContextOnOutput(string obj)
        {
            _dispatcher.Invoke(() => Console = Console + Environment.NewLine + obj, DispatcherPriority.Render);
        }

        private async Task OpenWebsite(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch(Exception e)
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") {CreateNoWindow = true});
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    Process.Start("xdg-open", url);
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    Process.Start("open", url);
                else
                {
                    LogTo.Error(e, "Error on Opening Net core Website");
                    await _messageService.ShowErrorAsync(e);
                }
            }
        }
    }
}