using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Catel.Collections;
using Catel.Data;
using Catel.MVVM;
using Catel.Services;
using Scrutor;
using Serilog.Context;
using Tauron.Application.Deployment.AutoUpload.Models.Core;
using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Common;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;
using Tauron.Application.Logging;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.RemoveCommand
{
    [ServiceDescriptor(typeof(RemoveSelectProjectViewModel))]
    public class RemoveSelectProjectViewModel : OperationViewModel<RemoveCommandContext>
    {
        private readonly Settings _settings;
        private readonly ISLogger<RemoveSelectProjectViewModel> _logger;
        private readonly IMessageService _messageService;

        public RemoveSelectProjectViewModel(Settings settings, ISLogger<RemoveSelectProjectViewModel> logger, IMessageService messageService)
        {
            _settings = settings;
            _logger = logger;
            _messageService = messageService;
            Projects.AddItems(_settings.RegistratedRepositories);

            NextCommand = new TaskCommand(OnNextCommandExecute, OnNextCommandCanExecute);
        }

        public FastObservableCollection<RegistratedRepository> Projects { get; } = new FastObservableCollection<RegistratedRepository>();

        public RegistratedRepository? SelectedProject { get; set; }


        public TaskCommand NextCommand { get; }

        private async Task OnNextCommandExecute()
        {
            using (LogContext.PushProperty("Repository", $"{SelectedProject?.RepositoryName}--{SelectedProject?.ProjectName}"))
            {
                try
                {
                    var repo = SelectedProject;
                    if (repo == null) return;

                    await _settings.RemoveProjecktAndSave(repo);

                    var path = repo.RealPath;
                    if (_settings.RegistratedRepositories.All(rr => rr.RealPath != path))
                    {
                        var info = new DirectoryInfo(path);
                        if (info.Exists)
                        {
                            SetAttributesNormal(info);
                            info.Delete(true);
                        }
                    }

                    await OnNextView<CommonFinishViewModel, FinishContext>(FinishContext.Default);
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error on Deleting Repository");
                    await _messageService.ShowErrorAsync(e);
                }
            }
        }


        private void SetAttributesNormal(DirectoryInfo dir)
        {
            _logger.Information("Set File Attributes: {Directory}", dir);
            foreach (var subDir in dir.GetDirectories())
                SetAttributesNormal(subDir);
            foreach (var file in dir.GetFiles()) file.Attributes = FileAttributes.Normal;
        }

        private bool OnNextCommandCanExecute() => SelectedProject != null;

        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            if (SelectedProject == null)
                validationResults.Add(FieldValidationResult.CreateError(nameof(SelectedProject), "Kein Projekt gewählt"));

            base.ValidateFields(validationResults);
        }
    }
}