using System;
using System.ComponentModel;
using System.Linq;
using Tauron.Application.Models;
using Tauron.Application.ProjectManager.Generic;
using Tauron.Application.ProjectManager.Services;
using Tauron.Application.ProjectManager.Services.Data.Entitys;

namespace Tauron.Application.MgiProjectManager.UI.Model
{
    [ExportModel(AppConststands.CreationWorkItemModel)]
    public sealed class CreationWorkItemModel : ModelBase
    {

        public static readonly ObservableProperty CurrentJobProperty = RegisterProperty(nameof(CurrentJob), typeof(CreationWorkItemModel), typeof(JobItem),
                                                                                        new ObservablePropertyMetadata(CurrentJobValueChanged));

        public static readonly ObservableProperty JobCreationProperty = RegisterProperty(nameof(JobCreation), typeof(CreationWorkItemModel), typeof(bool),
                                                                                         new ObservablePropertyMetadata((object) false));

        private ServiceManager _serviceManager;

        private bool _lock;

        private JobItem _newJob;

        public bool JobCreation
        {
            get => GetValue<bool>(JobCreationProperty);
            private set => SetValue(JobCreationProperty, value);
        }

        public JobItem CurrentJob
        {
            get => GetValue<JobItem>(CurrentJobProperty);
            set => SetValue(CurrentJobProperty, value);
        }
        
        public IJobManager JobManager { get; private set; }

        private static void CurrentJobValueChanged(ObservableProperty prop, ModelBase model, object value)
        {
            var itemModel = (CreationWorkItemModel) model;
            itemModel.OnJobValueChanged(value);
        }

        public event EventHandler<JobItemStatusEventArgs> ItemStatusChange;

        public void CreateJob()
        {
            CurrentJob = null;

            _lock       = true;
            JobCreation = true;

            _newJob = new JobItem
                      {
                          Name       = "BM_" + DateTime.Now.Year.ToString().Substring(2),
                          Status     = JobStatus.Creation,
                          TargetDate = DateTime.Now + TimeSpan.FromHours(48)
                      };

            SetValue(CurrentJobProperty, _newJob);
            OnItemStatusChange();
            _lock = false;
        }

        public void FinilizeCreation()
        {
            _lock       = true;
            _newJob     = null;
            JobCreation = false;
            _lock = false;
        }
        
        public void OnItemStatusChange()
        {
            ItemStatusChange?.Invoke(this, new JobItemStatusEventArgs(CurrentJob));
        }

        private void OnJobValueChanged(object value)
        {
            if (ReferenceEquals(_newJob, value)) return;

            if (!JobCreation)
            {
                if (_lock) return;

                OnItemStatusChange();
                return;
            }

            _lock       = true;
            CurrentJob  = null;
            JobCreation = false;
            OnItemStatusChange();
            _lock = false;
        }

        public void MarkImportent()
        {
            if (CurrentJob == null) return;

            CurrentJob.Importent = true;
            JobManager.MarkImportent(CurrentJob.CreateDto());
        }

        public override void BuildCompled()
        {
            _serviceManager = new ServiceManager(ViewModelBase.Dialogs, ViewModelBase.MainWindow);
            JobManager = _serviceManager.CreateClint<IJobManager>();
        }
    }
}