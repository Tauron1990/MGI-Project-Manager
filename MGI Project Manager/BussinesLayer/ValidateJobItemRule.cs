using System.Linq;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;
using Tauron.Application.Common.BaseLayer.Data;
using Tauron.Application.MgiProjectManager.Data.Entitys;
using Tauron.Application.MgiProjectManager.ServiceLayer.Dto;

namespace Tauron.Application.MgiProjectManager.BussinesLayer
{
    [ExportRule(RuleNames.ValidateJob)]
    public sealed class ValidateJobItemRule : IBusinessRuleBase<JobItemDto>
    {
        public override void ActionImpl(JobItemDto input)
        {
            using (RepositoryFactory.Enter())
            {
                var repo = RepositoryFactory.GetRepository<Repository<JobEntity, string>>();
                var ent  = repo.Query().FirstOrDefault(e => e.Id == input.Name);

                if (!ValidationRuleHelper.AssertList(new[]
                                                     {
                                                         new AssertHelp(ent == null, "AssertJobIsAlreadyInsertet"),
                                                         new AssertHelp(!string.IsNullOrWhiteSpace(input.Name), "AssertJobNameInvalid"),
                                                         new AssertHelp(input.Name.Length == 10, "AssertJobNameInvalid")
                                                     }, out var msg))
                    SetError(msg);
            }
        }
    }
}