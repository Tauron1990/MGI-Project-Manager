using System;
using System.Collections.Generic;
using Syncfusion.Windows.Controls.Notification;
using Tauron.Application.Ioc;
using Tauron.Application.MgiProjectManager.UI;
using Tauron.Application.MgiProjectManager.UI.Model;
using Tauron.Application.Models;
using Tauron.Application.ProjectManager.Generic;
using Tauron.Application.ProjectManager.Resources;
using Tauron.Application.ProjectManager.Services.Data.Entitys;

namespace Tauron.Application.MgiProjectManager
{
    [ExportViewModel(AppConststands.MainWindowName)]
    public sealed class MainWindowViewModel : ViewModelBase
    {
        private Dictionary<JobStatus, string> _statusViews = new Dictionary<JobStatus, string>
        {
            {JobStatus.Creation, AppConststands.CreateJobControl}
        };

        private bool _logInSetted;
        private bool _connectionOk;
        private string _currentStatus;
        private IJobChange _viewContent;
        private string _nextLabel;

        [InjectModel(AppConststands.TaskManager)]
        public TaskManagerModel TaskManagerModel { get; set; }

        [InjectModel(AppConststands.JobManagerModel)]
        public JobManagerModel JobManagerModel { get; set; }

        [InjectModel(AppConststands.CreationJobItemModel)]
        public CreationJobItemModel CreationJobItemModel { get; set; }

        [Inject]
        public ServiceManager ServiceManager { get; set; }

        public string CurrentStatus
        {
            get => _currentStatus;
            set => SetProperty(ref _currentStatus, value);
        }

        public IJobChange ViewContent
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
        public void Close() => MainWindow?.Close();

        [CommandTarget]
        public bool CanReconnect()
        {
            if (ServiceManager.StatusOk) return false;

            return !TaskManagerModel.IsBusy || !_connectionOk;
        }

        [CommandTarget]
        public void Reconnect() => ConnectInternal();

        [CommandTarget]
        public void ShowLogin() => TaskManagerModel.RunTask(() => ServiceManager.ClientFactory.ShowLoginWindow(MainWindow, false).Wait(), MainUIResources.MainWindowModel_Label_Login);

        [CommandTarget]
        public bool CanCreateJob() => !CreationJobItemModel.JobCreation;

        [CommandTarget]
        public void CreateJob()
        {
            CreationJobItemModel.CreateJob();
            OnNextViewInternal(false, CreationJobItemModel.CurrentJob);
        }

        [CommandTarget]
        public bool CanMarkImportent()
        {
            if (CreationJobItemModel.JobCreation) return true;
            return JobManagerModel.SelectedItem != null;
        }

        [CommandTarget]
        public void MarkImportent()
        {
            if(JobManagerModel.MarkImportent()) return;
            ProcessError();
        }

        [CommandTarget]
        public bool CanNextView()
        {
            if (ViewContent == null)
                return false;

            var temp = ViewContent.CanNext();
            return temp == null || temp.Value;
        }

        [CommandTarget]
        public void NextView()
        {
            if (ViewContent.FinalizeNext())
            {
                if (string.IsNullOrEmpty(CurrentStatus))
                    CurrentStatus = MainUIResources.MainWindowViewModel_Label_FinlizeFailed;
                OnNextViewInternal(false, next:false);
            }
            else
                OnNextViewInternal(false);
        }

        [EventTarget]
        public void ClickCurrentJob() => OnNextViewInternal(true);

        public override void BuildCompled()
        {
            NextLabel = MainUIResources.Common_Label_Next;

            ServiceManager.ConnectionEstablished += (manager, type, arg3) =>
            {
                CurrentStatus = string.Empty;
                _connectionOk = true;
                TaskManagerModel.UnLock();
            };
            ServiceManager.OpenFailed += () => _connectionOk = false;

            ConnectInternal();
        }

        private void OnNextViewInternal(bool currentJob, JobItem item = null, bool next = true)
        {
            if (next)
            {
                JobItem targetItem = currentJob ? JobManagerModel.CurrentJob : JobManagerModel.SelectedItem;
                if (item != null)
                    targetItem = item;
                if(ViewContent != null)
                    ViewContent.ConnectFailed -= ViewContentOnConnectFailed;


                if (targetItem != null && _statusViews.TryGetValue(targetItem.Status, out var modelName))
                {
                    if (ResolveViewModel(modelName) is IJobChange model)
                    {
                        model.OnJobChange(targetItem);
                        ViewContent = model;
                        ViewContent.ConnectFailed += ViewContentOnConnectFailed;
                        NextLabel = !string.IsNullOrEmpty(model.CustomLabel) ? model.CustomLabel : MainUIResources.Common_Label_Next;
                        return;
                    }
                }
            }

            ViewContent = null;
            NextLabel = MainUIResources.Common_Label_Next;
        }

        private void ViewContentOnConnectFailed(object sender, EventArgs e) => ProcessError();

        private void ConnectInternal()
        {
            TaskManagerModel.RunTask(ConnectRun, MainUIResources.MainWindowModel_Label_Connecting, AnimationTypes.GPS)
                .ContinueWith(t =>
                {
                    if(_connectionOk) return;
                    ProcessError();
                });
        }

        private void ProcessError()
        {
            string msg = ServiceManager.ProcessDefaultErrors();
          
            TaskManagerModel.Lock(MainUIResources.MainWindowModel_Label_ConnectionFailed, AnimationTypes.GPS);
            CurrentStatus = msg;
        }

        private void ConnectRun()
        {
            if (!_logInSetted)
            {
                if (ServiceManager.ClientFactory.ShowLoginWindow(MainWindow, false).Result)
                    _logInSetted = true;
                else
                {
                    _connectionOk = false;
                    return;
                }
            }

            _connectionOk = JobManagerModel.Connect();
        }
    }
}