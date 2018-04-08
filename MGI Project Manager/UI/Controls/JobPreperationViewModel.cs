using Tauron.Application.MgiProjectManager.UI.Model;
using Tauron.Application.Models;

namespace Tauron.Application.MgiProjectManager.UI.Controls
{
    [ExportViewModel(AppConststands.JobPreperation)]
    public class JobPreperationViewModel : ViewModelBase, IJobChange
    {
        [InjectModel(AppConststands.TimeCalculationModel)]
        public TimeCalculationModel TimeCalculationModel { get; set; }

        public void OnJobChange(JobItem item) => TimeCalculationModel.ChangeJob(item);

        public bool FinalizeNext()
        {
            return false;
        }

        [CommandTarget]
        public void CalculateTime()
        {
            TimeCalculationModel.Calculate();
        }

        [CommandTarget]
        public bool CanCalculateTime()
        {
            return TimeCalculationModel.CanCalculate();
        }

        public bool? CanNext()
        {
            return TimeCalculationModel.CanSave();
        }

        public string CustomLabel { get; }
    }
}