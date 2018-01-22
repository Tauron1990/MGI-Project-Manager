using Tauron.Application.Models;

namespace Tauron.Application.MgiProjectManager.UI.Controls
{
    [ExportViewModel(AppConststands.JobPreperation)]
    public class JobPreperationViewModel : ViewModelBase, IJobChange
    {
        public void OnJobChange(JobItem item)
        {
        }

        public bool FinalizeNext()
        {
            return false;
        }

        public bool? CanNext()
        {
            return false;
        }

        public string CustomLabel { get; }
    }
}