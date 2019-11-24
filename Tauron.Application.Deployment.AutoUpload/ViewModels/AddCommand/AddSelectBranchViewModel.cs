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
            NextCommand = new TaskCommand(OnNextCommandExecute, OnNextCommandCanExecute);
            IsLoading = true;
        }

        public TaskCommand NextCommand { get; }

        private bool OnNextCommandCanExecute() 
            => IsReady && SelectedBrnach != null;

        private async Task OnNextCommandExecute()
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