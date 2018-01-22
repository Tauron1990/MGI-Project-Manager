using System.Linq;
using Microsoft.EntityFrameworkCore;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;
using Tauron.Application.Common.BaseLayer.Data;
using Tauron.Application.MgiProjectManager.Data.Entitys;
using Tauron.Application.MgiProjectManager.ServiceLayer.Dto;

namespace Tauron.Application.MgiProjectManager.BussinesLayer
{
    [ExportRule(RuleNames.TimeCalculationFetchJobInfo)]
    public sealed class TimeCalculationFetchJobInfoRule : IOBusinessRuleBase<JobItemDto, JobRunDto>
    {
        public override JobRunDto ActionImpl(JobItemDto input)
        {
            using (RepositoryFactory.Enter())
            {
                var repo = RepositoryFactory.GetRepository<Repository<JobEntity, string>>();

                var ent = repo.Query().AsNoTracking().Include(e => e.JobRun)
                              .Where(e => e.Id == input.Name)
                              .Select(e => e.JobRun)
                              .Single();

                return new JobRunDto(ent.Iterations, ent.Amount, ent.Length, ent.Width, ent.Speed, ent.EffectiveTime, ent.IsValid);
            }
        }
    }
}