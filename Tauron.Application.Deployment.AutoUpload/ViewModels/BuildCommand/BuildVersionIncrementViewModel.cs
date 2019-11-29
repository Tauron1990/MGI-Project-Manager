using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catel.Data;
using Catel.MVVM;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand
{
    [ServiceDescriptor(typeof(BuildVersionIncrementViewModel))]
    public class BuildVersionIncrementViewModel : OperationViewModel<BuildOperationContext>
    {
        public string? FileVersion { get; set; }

        public string? AssemblyVersion { get; set; }

        public string? OldFileVersion { get; private set; }

        public string? OldAssemblyVersion { get; private set; }

        public BuildVersionIncrementViewModel()
        {
            NextCommand = new TaskCommand(OnNextExecute, OnNextCanExecute);
        }
        
        protected override Task InitializeAsync()
        {
            BeginLoad();
            return Task.CompletedTask;
        }

        public TaskCommand NextCommand { get; private set; }
        
        private Task OnNextExecute()
        {
            throw new System.NotImplementedException();
        }

        private bool OnNextCanExecute() => !HasErrors;

        private async void BeginLoad()
        {

        }

        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            if(!Version.TryParse(FileVersion, out _))
                validationResults.Add(FieldValidationResult.CreateError(nameof(FileVersion), "Datei Version ist kein Korrekter Versions String"));

            if (!Version.TryParse(AssemblyVersion, out _))
                validationResults.Add(FieldValidationResult.CreateError(nameof(AssemblyVersion), "Assembly Version ist kein Korrekter Versions String"));
        }
    }
}