using System.Threading.Tasks;
using Catel.Collections;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Models.Core;
using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Deployment.AutoUpload.ViewModels.AddCommand;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand
{
    [ServiceDescriptor(typeof(BuildSelectProjectViewModel))]
    public sealed class BuildSelectProjectViewModel : OperationViewModel<BuildOperationContext>
    {
        private readonly Settings _settings;

        public FastObservableCollection<PotentialProjekt> Projekts { get; } = new FastObservableCollection<PotentialProjekt>();

        public PotentialProjekt? Projekt { get; set; }

        public BuildSelectProjectViewModel(Settings settings) 
            => _settings = settings;

        protected override Task InitializeAsync()
        {

            foreach (var repository in _settings.RegistratedRepositories) 
                Projekts.Add(new PotentialProjekt(repository.ToString(), async () => await BuildProjectAction(repository)));

            Projekts.Add(new PotentialProjekt("Neu...", NewProjectAction));

            return Task.CompletedTask;
        }

        private async Task BuildProjectAction(RegistratedRepository registratedRepository)
        {
            Context.RegistratedRepository = registratedRepository;
            await OnNextView<BuildVersionIncrementViewModel>();
        }

        private async Task NewProjectAction() 
            => await OnNextView<AddNameSelectorViewModel, AddCommandContext>(new AddCommandContext(), CreateRedirection<BuildVersionIncrementViewModel>());
    }
}