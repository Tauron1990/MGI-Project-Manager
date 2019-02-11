using System;
using Tauron.Application.Ioc;
using Tauron.Application.MgiProjectManager.LocalCache;
using Tauron.Application.Models;
using Tauron.Application.ProjectManager.Generic;
using Tauron.Application.ProjectManager.Resources;
using Tauron.Application.ProjectManager.Services;
using Tauron.Application.ProjectManager.Services.Data.Entitys;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.MgiProjectManager.UI.Model
{
    [ExportModel(AppConststands.CreationJobItemModel)]
    public sealed class CreationJobItemModel : ModelBase
    {
        private const string ConnectionFailedId = "ConnectionFailed";

        public static readonly ObservableProperty CurrentJobProperty = RegisterProperty(nameof(CurrentJob), typeof(CreationJobItemModel), typeof(JobItem));

        public static readonly ObservableProperty JobCreationProperty = RegisterProperty(nameof(JobCreation), typeof(CreationJobItemModel), typeof(bool),
            new ObservablePropertyMetadata((object) false));

        private bool _lock;

        private JobItem _newJob;

        [Inject]
        public ServiceManager ServiceManager { get; set; }

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
        
        public event EventHandler<JobItemStatusEventArgs> ItemStatusChange;

        public void CreateJob()
        {
            CurrentJob = null;

            _lock = true;
            JobCreation = true;

            _newJob = new JobItem
            {
                Name = "BM_" + DateTime.Now.Year.ToString().Substring(2),
                Status = JobStatus.Creation,
                TargetDate = DateTime.Now + TimeSpan.FromHours(48)
            };

            SetValue(CurrentJobProperty, _newJob);
            _lock = false;
        }

        public (bool CallOk, bool InsertOk) FinilizeCreation()
        {
            if (!CacheManager.TakeCare<IJobManager, bool, JobItemDto>(JobManager.InsertJob, _newJob.CreateDto(), out var ok))
                return (false, false);

            _lock = true;
            JobCreation = false;
            _lock = false;
            CurrentJob = null;

            return (true, ok);
        }

        private void OnJobValueChanged(object value)
        {
            if (ReferenceEquals(_newJob, value)) return;

            if (!JobCreation)
            {
                if (_lock) return;
                return;
            }

            _lock = true;
            CurrentJob = null;
            JobCreation = false;
            _lock = false;
        }

        public void MarkImportent()
        {
            if(CurrentJob == null) return;

            CurrentJob.Importent = true;
        }

        public string ValidateJob(JobItemDto dto)
        {
            bool ok = CacheManager.TakeCare<IJobManager, string, JobItemDto>(JobManager.ValidateJob, dto, out var result);

            return !ok ? ServiceErrorMessages.ConnectionFailed : result;
        }

        public override void BuildCompled() => JobManager = ServiceManager.CreateClint<IJobManager>();
    }
}