using System;
using System.Threading.Tasks;
using Tauron.Application.MgiProjectManager.Data;
using Tauron.Application.MgiProjectManager.Resources;
using Tauron.Application.MgiProjectManager.UI;
using Tauron.Application.MgiProjectManager.UI.Model;
using Tauron.Application.Models;

namespace Tauron.Application.MgiProjectManager
{
    [ExportViewModel(AppConststands.MainWindowName)]
    public sealed class MainWindowViewModel : ViewModelBase
    {
        private IJobChange _currentJobChange;
        private string     _nextLabel = UIResources.LabelCommonNext;
        private object     _viewContent;
        private bool       _viewVisible;

        [InjectModel(AppConststands.TaskManager, EnablePropertyInheritance = true)]
        public TaskManagerModel TaskManager { get; set; }

        [InjectModel(AppConststands.WorkItemModel, EnablePropertyInheritance = true)]
        public WorkItemModel WorkItemModel { get; set; }

        public bool ViewVisible
        {
            get => _viewVisible;
            set => SetProperty(ref _viewVisible, value);
        }

        public object ViewContent
        {
            get => _viewContent;
            set => SetProperty(ref _viewContent, value);
        }

        public string NextLabel
        {
            get => _nextLabel;
            set => SetProperty(ref _nextLabel, value);
        }

        [CommandTarget]
        public void CreateJob()
        {
            WorkItemModel.CreateJob();
        }

        [CommandTarget]
        public void NextView()
        {
            if (_currentJobChange?.FinalizeNext() ?? true)
                WorkItemModel.OnItemStatusChange();
        }

        [CommandTarget]
        public bool CanNextView()
        {
            return _currentJobChange?.CanNext() ?? ViewContent != null;
        }

        [CommandTarget]
        public void MarkImportent()
        {
            WorkItemModel.MarkImportent();
        }

        [CommandTarget]
        public bool CanMarkImportent()
        {
            return WorkItemModel.CurrentJob != null && !WorkItemModel.CurrentJob.Importent && WorkItemModel.CurrentJob.Status != JobStatus.Creation;
        }

        public override void BuildCompled()
        {
            WorkItemModel.ItemStatusChange += WorkItemModelOnItemStatusChange;
            TaskManager.RunTask(WorkItemModel.FetchJobs, UIResources.TaskFetchJobs);
        }

        private void WorkItemModelOnItemStatusChange(object sender, JobItemStatusEventArgs jobItemStatusEventArgs)
        {
            if (jobItemStatusEventArgs.Item == null) return;

            switch (jobItemStatusEventArgs.Item.Status)
            {
                case JobStatus.Creation:
                    ShowItem(AppConststands.CreateJobControl);
                    break;
                case JobStatus.Prepare:
                    ShowItem(AppConststands.JobPreperation);
                    break;
                case JobStatus.Pending:
                    break;
                case JobStatus.InProgress:
                    break;
                case JobStatus.Drying:
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private void ShowItem(string name)
        {
            ViewVisible       = false;
            var fadout        = Task.Delay(2000);
            var model         = ResolveViewModel(name);
            _currentJobChange = model as IJobChange;
            _currentJobChange?.OnJobChange(WorkItemModel.CurrentJob);
            NextLabel = _currentJobChange?.CustomLabel ?? UIResources.LabelCommonNext;

            CurrentDispatcher.Invoke(() => ViewContent = model);

            fadout.ContinueWith(t => ViewVisible = true);
        }
    }
}