using System;
using System.Linq;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;
using Tauron.Application.Common.BaseLayer.Data;
using Tauron.Application.MgiProjectManager.Data.Entitys;
using Tauron.Application.MgiProjectManager.ServiceLayer.Dto;
using Tauron.Application.MgiProjectManager.UI;

namespace Tauron.Application.MgiProjectManager.BussinesLayer
{
    [ExportRule(RuleNames.TimeCalculationAddSetupItems)]
    public sealed class AddSetupItemRule : IBusinessRuleBase<AddSetupInput>
    {
        public override void ActionImpl(AddSetupInput input)
        {
            using (var database = RepositoryFactory.Enter())
            {
                var repo = RepositoryFactory.GetRepository<Repository<SetupEntity, int>>();

                foreach (var item in input.Items.Where(i => i != null))
                {
                    AddSetupEntity(repo, item);
                }

                database.SaveChanges();
            }
        }

        private void AddSetupEntity(Repository<SetupEntity, int> database, AddSetupInputItem item)
        {
            SetupType type;
            switch (item.ItemType)
            {
                case RunTimeCalculatorItemType.Iteration:
                    type = SetupType.Iteration;
                    break;
                case RunTimeCalculatorItemType.Setup:
                    type = SetupType.Setup;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var time = item.CalculateDiffernce();
            if (time == null) return;

            database.Add(new SetupEntity {SetupType = type, Value = (int) time.Value.TotalMinutes, StartTime = item.StartTime});
        }
    }
}