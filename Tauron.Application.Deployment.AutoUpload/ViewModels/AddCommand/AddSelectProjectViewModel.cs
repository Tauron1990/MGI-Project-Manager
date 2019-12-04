using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Catel.Data;
using Catel.Services;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Models.Core;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Common;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.AddCommand
{
    [ServiceDescriptor(typeof(AddSelectProjectViewModel))]
    public class AddSelectProjectViewModel : OperationViewModel<AddCommandContext>
    {
        private readonly Settings _settings;
        private readonly IMessageService _messageService;

        //[NoWeaving]
        //public FastObservableCollection<ProjectUI> Projects { get; } = new FastObservableCollection<ProjectUI>();

        //public ProjectUI? SelectedProject { get; set; }

        public ICommonSelectorViewModel ProjectSelector { get; } = CommonSelectorViewModel.Create();

        public AddSelectProjectViewModel(Settings settings, IMessageService messageService)
        {
            _settings = settings;
            _messageService = messageService;
        }

        protected override async Task InitializeAsync()
        {
            var projects = Directory.EnumerateFiles(Context.RealPath, "*.csproj", new EnumerationOptions {IgnoreInaccessible = true, RecurseSubdirectories = true})
                .Where(f => _settings.RegistratedRepositories.All(rr => rr.ProjectName != f))
                .Select(s => new ProjectUI(s));

            ProjectSelector.Init(projects.Select(ui => new SelectorItem<ProjectUI>(ui)), false, OnNext);
            await base.InitializeAsync();
        }

        private async Task OnNext(SelectorItemBase selectorItemBase)
        {
            try
            {
                if (!(selectorItemBase is SelectorItem<ProjectUI> selectedProject))
                    return;

                await _settings.AddProjecktAndSave(Context.CreateRegistratedRepository(selectedProject.Target.File));
                await OnNextView<CommonFinishViewModel, FinishContext>(new FinishContext("Das Projekt wurde Erfolgreich Hinzugefügt."));
            }
            catch (Exception e)
            {
                await _messageService.ShowErrorAsync(e);
                OnCancelOperation();
            }
        }

        [CommandTarget]
        private bool CanOnNext() 
            => ProjectSelector.CanRun();

        [CommandTarget]
        private async Task OnNext() 
            => await ProjectSelector.Run();

        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            if(ProjectSelector.CanRun())
                validationResults.Add(FieldValidationResult.CreateError(nameof(ProjectSelector), "Kein Projekt gewählt"));

            base.ValidateFields(validationResults);
        }
    }
}