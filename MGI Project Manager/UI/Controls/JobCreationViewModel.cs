using Tauron.Application.MgiProjectManager.Data;
using Tauron.Application.MgiProjectManager.Resources;
using Tauron.Application.MgiProjectManager.UI.Model;
using Tauron.Application.Models;

namespace Tauron.Application.MgiProjectManager.UI.Controls
{
    [ExportViewModel(AppConststands.CreateJobControl)]
    public class JobCreationViewModel : ViewModelBase, IJobChange
    {
        private string  _errorText;
        private JobItem _jobItem;

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

        [InjectModel(AppConststands.WorkItemModel)]
        public WorkItemModel WorkItemModel { get; set; }

        public void OnJobChange(JobItem item)
        {
            JobItem = item;
        }

        public bool FinalizeNext()
        {
            _jobItem.Status     = JobStatus.Prepare;
            _jobItem.TargetDate = _jobItem.TargetDate.Date;

            WorkItemModel.FinilizeCreation();

            return WorkItemModel.JobManager.InsertJob(_jobItem.CreateDto());
        }

        public bool? CanNext()
        {
            var errorKey = WorkItemModel.JobManager.ValidateJob(_jobItem.CreateDto());

            ErrorText = string.IsNullOrWhiteSpace(errorKey) ? null : UIResources.ResourceManager.GetString(errorKey) ?? errorKey;

            return errorKey == null;
        }

        public string CustomLabel { get; } = UIResources.JobCreationNextLabel;
    }
}