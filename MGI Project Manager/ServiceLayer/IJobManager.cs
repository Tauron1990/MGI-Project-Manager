using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Tauron.Application.MgiProjectManager.ServiceLayer.Dto;

namespace Tauron.Application.MgiProjectManager.ServiceLayer
{
    public interface IJobManager
    {
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        IEnumerable<JobItemDto> GetActiveJobs();

        bool   InsertJob(JobItemDto     jobItem);
        string ValidateJob(JobItemDto   jobItem);
        void   MarkImportent(JobItemDto jobItem);
    }
}