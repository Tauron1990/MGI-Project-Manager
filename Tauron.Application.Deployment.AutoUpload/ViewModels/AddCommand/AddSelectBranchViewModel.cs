using System;
using System.Linq;
using System.Threading.Tasks;
using Catel.Services;
using Octokit;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Common;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.AddCommand
{
    [ServiceDescriptor(typeof(AddSelectBranchViewModel))]
    public sealed class AddSelectBranchViewModel : OperationViewModel<AddCommandContext>
    {
        private class BranchElement : INameable
        {
            public Branch Branch { get; }
            public string Name => Branch.Name;

            public BranchElement(Branch branch)
            {
                Branch = branch;
            }
        }

        private readonly IMessageService _messageService;
        private readonly RepositoryManager _manager;

        public bool IsReady { get; set; }

        public bool IsLoading { get; set; }

        public ICommonSelectorViewModel BranchSelector { get; set; }

        //[NoWeaving]
        //public FastObservableCollection<Branch> Branches { get; } = new FastObservableCollection<Branch>();

        //public Branch? SelectedBrnach { get; set; }

        public AddSelectBranchViewModel(IMessageService messageService, RepositoryManager manager)
        {
            _messageService = messageService;
            _manager = manager;
            IsLoading = true;

            BranchSelector = CommonSelectorViewModel.Create();
        }

        private async Task OnNextImpl(SelectorItemBase item)
        {
            if (!(item is SelectorItem<BranchElement> selectedBrnach)) return;

            Context.Branch = selectedBrnach.Target.Branch;
            await OnNextView<AddSyncRepositoryViewModel>();
        }

        [CommandTarget]
        private bool CanOnNext() => IsReady && BranchSelector.CanRun();

        [CommandTarget]
        private async Task OnNext() 
            => await BranchSelector.Run();

        protected override Task InitializeAsync()
        {
            BeginLoad();
            return base.InitializeAsync();
        }

        private async void BeginLoad()
        {
            try
            {
                BranchSelector.Init((await _manager.GetBranchs(Context.Repository)).Select(e => new SelectorItem<BranchElement>(new BranchElement(e)))
                , false, OnNextImpl);
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