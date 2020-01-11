using System;
using System.Threading;
using System.Threading.Tasks;
using Catel.Services;
using Scrutor;
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

        public int CurrentValue { get; set; }

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
            
        }

        private void AddConsole(string value) 
            => Console = $"{Console}{Environment.NewLine}{value}";

        public void Dispose() => _cancel.Dispose();
    }
}