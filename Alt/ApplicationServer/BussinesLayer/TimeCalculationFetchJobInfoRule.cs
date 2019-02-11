using System.Linq;
using Microsoft.EntityFrameworkCore;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;
using Tauron.Application.ProjectManager.ApplicationServer.Data.Repositorys;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.ApplicationServer.BussinesLayer
{
    [ExportRule(RuleNames.TimeCalculationFetchJobInfo)]
    public sealed class TimeCalculationFetchJobInfoRule : IOBusinessRuleBase<JobItemDto, JobRunDto[]>
    {
        public override JobRunDto[] ActionImpl(JobItemDto input)
        {
            using (RepositoryFactory.Enter())
            {
                var repo = RepositoryFactory.GetRepository<IJobRepository>();

                var ent = repo.Query().AsNoTracking().Include(e => e.JobRuns)
                              .Where(e => e.Id == input.Name)
                              .Select(e => e.JobRuns)
                              .Single();

                return ent.Select(JobRunDto.FromEntity).ToArray();
            }
        }
    }
}