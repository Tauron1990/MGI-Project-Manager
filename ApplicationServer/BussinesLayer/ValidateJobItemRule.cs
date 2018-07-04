using System.Linq;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;
using Tauron.Application.ProjectManager.ApplicationServer.Data.Repositorys;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.ApplicationServer.BussinesLayer
{
    [ExportRule(RuleNames.ValidateJob)]
    public sealed class ValidateJobItemRule : IBusinessRuleBase<JobItemDto>
    {
        public override void ActionImpl(JobItemDto input)
        {
            using (RepositoryFactory.Enter())
            {
                var repo = RepositoryFactory.GetRepository<IJobRepository>();
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