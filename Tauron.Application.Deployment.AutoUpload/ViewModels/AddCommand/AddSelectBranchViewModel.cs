using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Catel.Collections;
using Catel.Fody;
using Catel.MVVM;
using Catel.Services;
using Octokit;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.AddCommand
{
    [ServiceDescriptor(typeof(AddSelectBranchViewModel))]
    public sealed class AddSelectBranchViewModel : OperationViewModel<AddCommandContext>
    {
        private readonly IMessageService _messageService;
        private readonly RepositoryManager _manager;

        public bool IsReady { get; set; }

        public bool IsLoading { get; set; }

        [NoWeaving]
        public FastObservableCollection<Branch> Branches { get; } = new FastObservableCollection<Branch>();

        public Branch? SelectedBrnach { get; set; }

        public AddSelectBranchViewModel(IMessageService messageService, RepositoryManager manager)
        {
            _messageService = messageService;
            _manager = manager;
            IsLoading = true;
        }

        [CommandTarget]
        private bool CanOnNext() => IsReady && SelectedBrnach != null;

        [CommandTarget]
        private async Task OnNext()
        {
            if(SelectedBrnach == null) return;

            Context.Branch = SelectedBrnach;
            await OnNextView<AddSyncRepositoryViewModel>();
        }

        protected override Task InitializeAsync()
        {
            BeginLoad();
            return base.InitializeAsync();
        }

        private async void BeginLoad()
        {
            try
            {
                Branches.AddItems(await _manager.GetBranchs(Context.Repository));
            }
            catch (Exception e)
            {
                await _messageService.ShowErrorAsync(e);
            }
            finally
            {
                IsReady = true;
                IsLoading = false;
            }
        }
    }
}