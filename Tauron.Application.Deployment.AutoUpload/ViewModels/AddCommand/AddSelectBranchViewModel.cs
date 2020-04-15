using System;
using System.Linq;
using System.Threading.Tasks;
using Anotar.Serilog;
using Catel.Services;
using Octokit;
using Scrutor;
using Serilog.Context;
using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Common;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.AddCommand
{
    [ServiceDescriptor(typeof(AddSelectBranchViewModel))]
    public sealed class AddSelectBranchViewModel : OperationViewModel<AddCommandContext>
    {
        private readonly RepositoryManager _manager;

        private readonly IMessageService _messageService;

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

        public bool IsReady { get; set; }

        public bool IsLoading { get; set; }

        public ICommonSelectorViewModel BranchSelector { get; set; }

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
        {
            await BranchSelector.Run();
        }

        protected override Task InitializeAsync()
        {
            BeginLoad();
            return base.InitializeAsync();
        }

        private async void BeginLoad()
        {
            using (LogContext.PushProperty("Repository", Context.RegistratedRepository?.ToString()))
            {
                try
                {
                    LogTo.Information("Begin Loading Branches");
                    BranchSelector.Init((await _manager.GetBranchs(Context.Repository)).Select(e => new SelectorItem<BranchElement>(new BranchElement(e)))
                      , false, OnNextImpl);
                }
                catch (Exception e)
                {
                    LogTo.Error(e, "Error on loding Branches");
                    await _messageService.ShowErrorAsync(e);
                }
                finally
                {
                    IsReady = true;
                    IsLoading = false;
                }
            }
        }

        private class BranchElement : INameable
        {
            public BranchElement(Branch branch) => Branch = branch;

            public Branch Branch { get; }
            public string Name => Branch.Name;
        }
    }
}