using System.Linq;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;
using Tauron.Application.ProjectManager.ApplicationServer.Data.Repositorys;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.ApplicationServer.BussinesLayer
{
    [ExportRule(RuleNames.MarkImportent)]
    public sealed class MarkImportentRule : IBusinessRuleBase<JobItemDto>
    {
        public override void ActionImpl(JobItemDto input)
        {
            using (var db = RepositoryFactory.Enter())
            {
                var repo = RepositoryFactory.GetRepository<IJobRepository>();

                var item = repo.Query().SingleOrDefault(e => e.Id == input.Name);
                if (item == null) return;

                item.Importent = true;

                db.SaveChanges();
            }
        }
    }
}