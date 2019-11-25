using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Catel.Collections;
using Catel.Data;
using Catel.MVVM;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Models.Core;
using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Common;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.RemoveCommand
{
    [ServiceDescriptor(typeof(RemoveSelectProjectViewModel))]
    public class RemoveSelectProjectViewModel : OperationViewModel<RemoveCommandContext>
    {
        private readonly Settings _settings;

        public FastObservableCollection<RegistratedRepository> Projects { get; } = new FastObservableCollection<RegistratedRepository>();

        public RegistratedRepository? SelectedProject { get; set; }

        public RemoveSelectProjectViewModel(Settings settings)
        {
            _settings = settings;
            Projects.AddItems(_settings.RegistratedRepositories);

            NextCommand = new TaskCommand(OnNextCommandExecute, OnNextCommandCanExecute);
        }

        
        public TaskCommand NextCommand { get; }

        private async Task OnNextCommandExecute()
        {
            var repo = SelectedProject;
            if (repo == null) return;

            var path = repo.RealPath;
            if(_settings.RegistratedRepositories.All(rr => rr.RealPath != path))
                Directory.Delete(path, true);

            await _settings.RemoveProjecktAndSave(repo);

            await OnNextView<CommonFinishViewModel, FinishContext>(FinishContext.Default);
        }

        private bool OnNextCommandCanExecute() 
            => SelectedProject != null;

        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            if(SelectedProject == null)
                validationResults.Add(FieldValidationResult.CreateError(nameof(SelectedProject), "Kein Projekt gewählt"));

            base.ValidateFields(validationResults);
        }
    }
}