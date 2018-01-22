using System.Linq;
using Microsoft.EntityFrameworkCore;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;
using Tauron.Application.Common.BaseLayer.Data;
using Tauron.Application.MgiProjectManager.Data.Entitys;
using Tauron.Application.MgiProjectManager.Properties;

namespace Tauron.Application.MgiProjectManager.BussinesLayer
{
    [ExportRule(RuleNames.TimeCalculationRecalculateSetup)]
    public sealed class TimeCalculationRecalculateSetupRule : BusinessRuleBase
    {
        public override void ActionImpl()
        {
            using (RepositoryFactory.Enter())
            {
                var repo = RepositoryFactory.GetRepository<Repository<SetupEntity, int>>();

                foreach (var group in repo.Query().AsNoTracking().GroupBy(e => e.SetupType))
                {
                    var time = group.Average(e => e.Value);

                    switch (group.Key)
                    {
                        case SetupType.Setup:
                            Settings.Default.SetupTime = (int) time;
                            break;
                        case SetupType.Iteration:
                            Settings.Default.IterationTime = (int) time;
                            break;
                    }
                }
            }

            Settings.Default.Save();
        }
    }
}