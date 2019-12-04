using System.Linq;
using System.Threading.Tasks;
using Catel.Collections;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Models.Core;
using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Deployment.AutoUpload.ViewModels.AddCommand;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Common;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand
{
    [ServiceDescriptor(typeof(BuildSelectProjectViewModel))]
    public sealed class BuildSelectProjectViewModel : OperationViewModel<BuildOperationContext>
    {
        private readonly Settings _settings;

        //public FastObservableCollection<PotentialProjekt> Projekts { get; } = new FastObservableCollection<PotentialProjekt>();

        //public PotentialProjekt? Projekt { get; set; }

        public ICommonSelectorViewModel ProjectSelector { get; } = CommonSelectorViewModel.Create();

        public BuildSelectProjectViewModel(Settings settings) 
            => _settings = settings;

        protected override Task InitializeAsync()
        {
            var itemFac = _settings
                .RegistratedRepositories
                .Select(rr => new PotentialProjekt(rr))
                .Select(pr => new SelectorItem<PotentialProjekt>(pr));

            ProjectSelector.Init(itemFac, true, OnNext);
            
            return Task.CompletedTask;
        }

        private async Task OnNext(SelectorItemBase arg)
        {
            switch (arg)
            {
                case NewSelectorItem _:
                    await NewProjectAction();
                    break;
                case SelectorItem<PotentialProjekt> item:
                    await BuildProjectAction(item.Target.Repository);
                    break;
            }
        }

        private async Task BuildProjectAction(RegistratedRepository registratedRepository)
        {
            Context.RegistratedRepository = registratedRepository;
            await OnNextView<BuildVersionIncrementViewModel>();
        }

        private async Task NewProjectAction() 
            => await OnNextView<AddNameSelectorViewModel, AddCommandContext>(new AddCommandContext(), CreateRedirection<BuildVersionIncrementViewModel>());

        [CommandTarget]
        public bool CanOnNext() => ProjectSelector.CanRun();

        [CommandTarget]
        public async Task OnNext() 
            => await ProjectSelector.Run();
    }
}