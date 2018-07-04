using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;
using Tauron.Application.ProjectManager.ApplicationServer.Data.Repositorys;
using Tauron.Application.ProjectManager.Services.Data.Entitys;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.ApplicationServer.BussinesLayer
{
    [ExportRule(RuleNames.InsertJob)]
    public sealed class InsertJobRule : IOBusinessRuleBase<JobItemDto, bool>
    {
        public override bool ActionImpl(JobItemDto input)
        {
            using (var db = RepositoryFactory.Enter())
            {
                var repo = RepositoryFactory.GetRepository<IJobRepository>();

                repo.Add(new JobEntity
                         {
                             Id         = input.Name,
                             Importent  = input.Importent,
                             IsActive   = true,
                             LongName   = input.LongName,
                             TargetDate = input.TargetDate,
                             Status     = input.Status
                         });

                db.SaveChanges();

                return true;
            }
        }
    }
}