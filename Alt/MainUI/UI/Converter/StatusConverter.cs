using System.Windows.Data;
using Tauron.Application.Converter;
using Tauron.Application.ProjectManager.Resources;
using Tauron.Application.ProjectManager.Services.Data.Entitys;

namespace Tauron.Application.MgiProjectManager.UI.Converter
{
    public sealed class StatusConverter : ValueConverterFactoryBase
    {
        protected override IValueConverter Create()
        {
            return new Converter();
        }

        private class Converter : StringConverterBase<JobStatus>
        {
            protected override string Convert(JobStatus value)
            {
                switch (value)
                {
                    case JobStatus.Creation:
                        return MainUIResources.StatusConverter_StatusCreation;
                    case JobStatus.Prepare:
                        return MainUIResources.StatusConverter_StatusPrepare;
                    case JobStatus.Pending:
                        return MainUIResources.StatusConverter_StatusPending;
                    case JobStatus.InProgress:
                        return MainUIResources.StatusConverter_StatusInProgress;
                    case JobStatus.Drying:
                        return MainUIResources.StatusConverter_StatusDrying;
                    case JobStatus.Compled:
                        return MainUIResources.StatusConverter_StatusCompled;
                    default:
                        return "Unknowen";
                }
            }
        }
    }
}