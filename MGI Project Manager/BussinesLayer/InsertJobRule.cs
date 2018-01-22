using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;
using Tauron.Application.Common.BaseLayer.Data;
using Tauron.Application.MgiProjectManager.Data.Entitys;
using Tauron.Application.MgiProjectManager.ServiceLayer.Dto;

namespace Tauron.Application.MgiProjectManager.BussinesLayer
{
    [ExportRule(RuleNames.InsertJob)]
    public sealed class InsertJobRule : IOBusinessRuleBase<JobItemDto, bool>
    {
        public override bool ActionImpl(JobItemDto input)
        {
            using (var db = RepositoryFactory.Enter())
            {
                var repo = RepositoryFactory.GetRepository<Repository<JobEntity, string>>();

                repo.Add(new JobEntity
                         {
                             Id         = input.Name,
                             Importent  = input.Importent,
                             IsActive   = true,
                             LongName   = input.LongName,
                             TargetDate = input.TargetDate,
                             Status     = input.Status,
                             JobRun     = new JobRunEntity()
                         });

                db.SaveChanges();

                return true;
            }
        }
    }
}