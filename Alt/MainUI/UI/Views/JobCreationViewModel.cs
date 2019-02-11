using System;
using Tauron.Application.MgiProjectManager.Helper;
using Tauron.Application.MgiProjectManager.UI.Model;
using Tauron.Application.Models;
using Tauron.Application.ProjectManager.Resources;
using Tauron.Application.ProjectManager.Services.Data.Entitys;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.MgiProjectManager.UI.Views
{
    [ExportViewModel(AppConststands.CreateJobControl)]
    public class JobCreationViewModel : ViewModelBase, IJobChange
    {
        private readonly OperationHelper<JobItemDto, bool> _validateOperation;

        private string  _errorText;
        private JobItem _jobItem;
        private bool _canNext;

        public JobItem JobItem
        {
            get => _jobItem;
            set => SetProperty(ref _jobItem, value);
        }

        public string ErrorText
        {
            get => _errorText;
            set => SetProperty(ref _errorText, value);
        }

        [InjectModel(AppConststands.CreationJobItemModel)]
        public CreationJobItemModel CreationJobItemModel { get; set; }

        public JobCreationViewModel()
        {
            _validateOperation = new OperationHelper<JobItemDto, bool>(() => JobItem.CreateDto(), ValidateItem, b =>
            {
                _canNext = b;
                InvalidateRequerySuggested();
            });
        }

        private bool ValidateItem(JobItemDto arg)
        {
            var errorKey = CreationJobItemModel.ValidateJob(arg);

            ErrorText = string.IsNullOrWhiteSpace(errorKey) ? null : ServiceErrorMessages.ResourceManager.GetString(errorKey) ?? errorKey;

            return errorKey == null;
        }

        public void OnJobChange(JobItem item)
        {
            JobItem = item;
        }

        public bool FinalizeNext()
        {
            var result = CreationJobItemModel.FinilizeCreation();

            if (!result.CallOk)
            {
                OnConnectFailed();
                return false;
            }

            if(result.InsertOk)
                _jobItem.Status     = JobStatus.Prepare;
                _jobItem.TargetDate = _jobItem.TargetDate.Date;

            return result.InsertOk;
        }

        public bool? CanNext()
        {
            _validateOperation.Run();
            return _canNext;
        }

        public event EventHandler ConnectFailed;
        public string CustomLabel { get; } = MainUIResources.JobCreationViewModel_Label_Next;

        protected void OnConnectFailed()
        {
            ConnectFailed?.Invoke(this, EventArgs.Empty);
        }
    }
}