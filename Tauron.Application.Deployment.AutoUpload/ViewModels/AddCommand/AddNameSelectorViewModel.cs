using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catel.Data;
using Catel.Fody;
using Catel.MVVM;
using Catel.Services;
using Octokit;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Models.Core;
using Tauron.Application.Deployment.AutoUpload.Models.Github;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.AddCommand
{
    [ServiceDescriptor(typeof(AddNameSelectorViewModel))]
    public class AddNameSelectorViewModel : OperationViewModel<AddCommandContext>
    {
        private readonly RepositoryManager _repositoryManager;
        private readonly IMessageService _messageService;
        private string _selectedRepo = string.Empty;

        public Settings Settings { get; }

        [NoWeaving]
        public string SelectedRepo
        {
            get => _selectedRepo;
            set
            {
                if (Equals(value, _selectedRepo)) return;
                _selectedRepo = value;
                RepositoryName = _selectedRepo;
                RaisePropertyChanged(nameof(SelectedRepo));
            }
        }

        public string RepositoryName { get; set; } = string.Empty;

        public bool IsFetching { get; set; }

        public AddNameSelectorViewModel(Settings settings, RepositoryManager repositoryManager, IMessageService messageService)
        {
            _repositoryManager = repositoryManager;
            _messageService = messageService;
            Settings = settings;
            NextCommand = new TaskCommand(OnNextCommandExecute, OnNextCommandCanExecute);
        }



        public TaskCommand NextCommand { get; }

        private bool OnNextCommandCanExecute() => !HasErrors;

        private async Task OnNextCommandExecute()
        {
            try
            {
                IsFetching = true;

                var repository = await _repositoryManager.GetRepository(RepositoryName);
                await Settings.AddRepoAndSave(repository.FullName);

                Context.Repository = repository;
                await OnNextView<AddSelectBranchViewModel>();
            }
            catch (Exception e)
            {
                await _messageService.ShowErrorAsync(e);
            }
            finally
            {
                IsFetching = false;
            }
        }

        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            if (string.IsNullOrWhiteSpace(RepositoryName)) 
                validationResults.Add(FieldValidationResult.CreateError(nameof(RepositoryName), "Repository darf nicht Leer sein"));
            else if(RepositoryName.Split('/').Length != 2)
                validationResults.Add(FieldValidationResult.CreateError(nameof(RepositoryName), $"\"{RepositoryName}\" hat das Falsche Format"));

            base.ValidateFields(validationResults);
        }
    }
}