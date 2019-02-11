using System.Linq;
using Microsoft.EntityFrameworkCore;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;
using Tauron.Application.ProjectManager.ApplicationServer.Core;
using Tauron.Application.ProjectManager.ApplicationServer.Data.Repositorys;
using Tauron.Application.ProjectManager.Services.Data.Entitys;

namespace Tauron.Application.ProjectManager.ApplicationServer.BussinesLayer
{
    [ExportRule(RuleNames.TimeCalculationRecalculateSetup)]
    public sealed class TimeCalculationRecalculateSetupRule : BusinessRuleBase
    {
        public override void ActionImpl()
        {
            using (RepositoryFactory.Enter())
            {
                var repo = RepositoryFactory.GetRepository<ISetupRepository>();

                foreach (var group in repo.Query().AsNoTracking().GroupBy(e => e.SetupType))
                {
                    var time = group.Average(e => e.Value);

                    switch (group.Key)
                    {
                        case SetupType.Setup:
                            SettingsManager.Default.SetupTime = (int) time;
                            break;
                        case SetupType.Iteration:
                            SettingsManager.Default.IterationTime = (int) time;
                            break;
                    }
                }
            }
        }
    }
}