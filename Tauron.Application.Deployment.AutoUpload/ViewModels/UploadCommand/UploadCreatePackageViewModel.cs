using System;
using System.Threading;
using System.Threading.Tasks;
using Catel.Services;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.UploadCommand
{
    [ServiceDescriptor(typeof(UploadCreatePackageViewModel))]
    public class UploadCreatePackageViewModel : OperationViewModel<UploadCommandContext>, IDisposable
    {
        private readonly IMessageService _messageService;
        private readonly CancellationTokenSource _cancel = new CancellationTokenSource();

        public string Console { get; set; } = string.Empty;

        private bool IsFailed { get; set; }

        public UploadCreatePackageViewModel(IMessageService messageService) 
            => _messageService = messageService;

        [CommandTarget]
        public void CancelOp() 
            => _cancel.Cancel();

        [CommandTarget]
        public bool CanCancelOp() 
            => !_cancel.IsCancellationRequested;

        protected override Task InitializeAsync()
        {
            Task.Run(StartOperation);
            return base.InitializeAsync();
        }

        private async void StartOperation()
        {
            AddConsole("Überprüfe Build");

            if (Context.Output == null)
            {
                await _messageService.ShowErrorAsync("Kein Build Gefunden");
                await OnReturn();
                return;
            }

            await Context.Output.Do(Run, Faild);
        }

        private async Task Faild(BuildFailed arg)
        {
            Console = $"Build Fehlerhaft:{Environment.NewLine}{arg.Console}{Environment.NewLine}Prozess Benende mit: {arg.Result} Code -- Fehlerzahl: {arg.ErrorCount}";
            IsFailed = true;
            await _messageService.ShowErrorAsync()
        }

        private Task Run(string arg1, Version arg2)
        {
            throw new NotImplementedException();
        }

        private void AddConsole(string value) 
            => Console = $"{Console}{Environment.NewLine}{value}";

        public void Dispose() => _cancel.Dispose();
    }
}