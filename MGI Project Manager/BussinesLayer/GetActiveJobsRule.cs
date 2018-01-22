using System.Collections.Generic;
using System.Linq;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;
using Tauron.Application.Common.BaseLayer.Data;
using Tauron.Application.MgiProjectManager.Data.Entitys;
using Tauron.Application.MgiProjectManager.ServiceLayer.Dto;

namespace Tauron.Application.MgiProjectManager.BussinesLayer
{
    [ExportRule(RuleNames.GetActiveJobsRule)]
    public sealed class GetActiveJobsRule : OBuissinesRuleBase<IEnumerable<JobItemDto>>
    {
        public override IEnumerable<JobItemDto> ActionImpl()
        {
            using (RepositoryFactory.Enter())
            {
                var repo = RepositoryFactory.GetRepository<Repository<JobEntity, string>>();

                return repo.Query()
                           .Where(jobEntity => jobEntity.IsActive)
                           .Select(jobEntity => new JobItemDto
                                                {
                                                    Importent  = jobEntity.Importent,
                                                    LongName   = jobEntity.LongName,
                                                    Name       = jobEntity.Id,
                                                    Status     = jobEntity.Status,
                                                    TargetDate = jobEntity.TargetDate
                                                })
                           .ToArray();
            }
        }
    }
}