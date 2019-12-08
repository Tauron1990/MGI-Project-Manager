﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Catel.Data;
using Catel.MVVM;
using Catel.Services;
using Scrutor;
using Tauron.Application.Deployment.AutoUpload.Models.Build;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Operations;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand
{
    [ServiceDescriptor(typeof(BuildVersionIncrementViewModel))]
    public class BuildVersionIncrementViewModel : OperationViewModel<BuildOperationContext>
    {
        private readonly IMessageService _messageService;
        private readonly ProjectFile _projectFile = new ProjectFile();

        private Version _internalAssembly = new Version();

        private Version _internalFile = new Version();

        public string? FileVersion { get; set; }

        public string? AssemblyVersion { get; set; }

        public string? OldFileVersion { get; private set; }

        public string? OldAssemblyVersion { get; private set; }

        public BuildVersionIncrementViewModel(IMessageService messageService)
        {
            _messageService = messageService;
        }
        
        protected override Task InitializeAsync()
        {
            BeginLoad();
            return Task.CompletedTask;
        }

        [CommandTarget]
        private async Task OnNext()
        {
            try
            {
                await _projectFile.ApplyVersion(_internalFile, _internalAssembly);
                await OnNextView<BuildBuildViewModel>();
            }
            catch (Exception e)
            {
                await _messageService.ShowErrorAsync(e);
                await OnReturn();
            }
        }

        [CommandTarget]
        private bool CanOnNext() => !HasErrors;

        private async void BeginLoad()
        {
            try
            {
                await _projectFile.Init(Context.RegistratedRepository?.ProjectName);

                _internalAssembly = _projectFile.GetAssemblyVersion();
                _internalFile = _projectFile.GetFileVersion();

                OldFileVersion = _internalFile.ToString();
                OldAssemblyVersion = _internalAssembly.ToString();

                await IncreaseAll();
            }
            catch (Exception e)
            {
                await _messageService.ShowErrorAsync($"Fehler: {e.GetType()}--{e.Message}");
                await OnReturn();
            }
        }

        [CommandTarget]
        public Task IncreaseAsm()
        {
            _internalAssembly = Increase(_internalAssembly);
            AssemblyVersion = _internalAssembly.ToString();

            //Validate(true);

            return Task.CompletedTask;
        }

        [CommandTarget]
        public Task IncreaseFile()
        {
            _internalFile = Increase(_internalFile);
            FileVersion = _internalFile.ToString();

            //Validate(true);

            return Task.CompletedTask;
        }

        [CommandTarget]
        public async Task IncreaseAll()
        {
            await IncreaseFile();
            await IncreaseAsm();
        }

        private static Version Increase(Version current)
        {
            int GraterThenZero(int input)
                => input < 0 ? 0 : input;

            current = new Version(current.Major, current.Minor + 1, GraterThenZero(current.Build), GraterThenZero(current.Revision));

            return current.Minor > 10 ? new Version(current.Major + 1, 0, GraterThenZero(current.Build), GraterThenZero(current.Revision)) : current;
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