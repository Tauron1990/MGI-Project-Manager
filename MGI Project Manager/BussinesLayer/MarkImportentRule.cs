using System.Linq;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;
using Tauron.Application.Common.BaseLayer.Data;
using Tauron.Application.MgiProjectManager.Data.Entitys;
using Tauron.Application.MgiProjectManager.ServiceLayer.Dto;

namespace Tauron.Application.MgiProjectManager.BussinesLayer
{
    [ExportRule(RuleNames.MarkImportent)]
    public sealed class MarkImportentRule : IBusinessRuleBase<JobItemDto>
    {
        public override void ActionImpl(JobItemDto input)
        {
            using (var db = RepositoryFactory.Enter())
            {
                var repo = RepositoryFactory.GetRepository<Repository<JobEntity, string>>();

                var item = repo.Query().SingleOrDefault(e => e.Id == input.Name);
                if (item == null) return;

                item.Importent = true;

                db.SaveChanges();
            }
        }
    }
}