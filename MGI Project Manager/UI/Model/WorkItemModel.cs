using System;
using System.ComponentModel;
using System.Linq;
using Tauron.Application.Ioc;
using Tauron.Application.MgiProjectManager.Data;
using Tauron.Application.MgiProjectManager.ServiceLayer;
using Tauron.Application.Models;

namespace Tauron.Application.MgiProjectManager.UI.Model
{
    [ExportModel(AppConststands.WorkItemModel)]
    public sealed class WorkItemModel : ModelBase
    {
        public static readonly ObservableProperty JobItemsProperty = RegisterProperty(nameof(JobItems), typeof(WorkItemModel), typeof(UISyncObservableCollection<JobItem>),
                                                                                      new ObservablePropertyMetadata(new UISyncObservableCollection<JobItem>()));

        public static readonly ObservableProperty CurrentJobProperty = RegisterProperty(nameof(CurrentJob), typeof(WorkItemModel), typeof(JobItem),
                                                                                        new ObservablePropertyMetadata(CurrentJobValueChanged));

        public static readonly ObservableProperty JobCreationProperty = RegisterProperty(nameof(JobCreation), typeof(WorkItemModel), typeof(bool),
                                                                                         new ObservablePropertyMetadata((object) false));

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

        public UISyncObservableCollection<JobItem> JobItems => GetValue<UISyncObservableCollection<JobItem>>(JobItemsProperty);

        public ICollectionView CollectionView { get; set; }

        [Inject]
        public IJobManager JobManager { get; private set; }

        private static void CurrentJobValueChanged(ObservableProperty prop, ModelBase model, object value)
        {
            var itemModel = (WorkItemModel) model;
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
                          Name       = "BM" + DateTime.Now.Year.ToString().Substring(2),
                          Status     = JobStatus.Creation,
                          TargetDate = DateTime.Now + TimeSpan.FromHours(48)
                      };

            JobItems.Add(_newJob);
            SetValue(CurrentJobProperty, _newJob);
            OnItemStatusChange();
            _lock = false;
        }

        public void FinilizeCreation()
        {
            _lock       = true;
            _newJob     = null;
            JobCreation = false;
            CurrentDispatcher.BeginInvoke(CollectionView.Refresh);
            _lock = false;
        }

        public void FetchJobs()
        {
            JobItems.Clear();

            JobItems.AddRange(JobManager.GetActiveJobs().Select(dto => new JobItem(dto)));
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
            JobItems.Remove(_newJob);
            OnItemStatusChange();
            _lock = false;
        }

        public void MarkImportent()
        {
            if (CurrentJob == null) return;

            CurrentJob.Importent = true;
            JobManager.MarkImportent(CurrentJob.CreateDto());
            CurrentDispatcher.BeginInvoke(CollectionView.Refresh);
        }
    }
}