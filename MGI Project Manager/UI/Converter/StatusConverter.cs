using System.Windows.Data;
using Tauron.Application.Converter;
using Tauron.Application.MgiProjectManager.Data;
using Tauron.Application.MgiProjectManager.Resources;

namespace Tauron.Application.MgiProjectManager.UI.Converter
{
    public sealed class StatusConverter : ValueConverterFactoryBase
    {
        private class Converter : StringConverterBase<JobStatus>
        {
            protected override string Convert(JobStatus value)
            {
                switch (value)
                {
                    case JobStatus.Creation:
                        return UIResources.StatusCreation;
                    case JobStatus.Prepare:
                        return UIResources.StatusPrepare;
                    case JobStatus.Pending:
                        return UIResources.StatusPending;
                    case JobStatus.InProgress:
                        return UIResources.StatusInProgress;
                    case JobStatus.Drying:
                        return UIResources.StatusDrying;
                    case JobStatus.Compled:
                        return UIResources.StatusCompled;
                    default:
                        return "Unknowen";
                }
            }
        }

        protected override IValueConverter Create()
        {
            return new Converter();
        }
    }
}