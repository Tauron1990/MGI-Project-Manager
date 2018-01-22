using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;
using Tauron.Application.MgiProjectManager.ServiceLayer.Dto;

namespace Tauron.Application.MgiProjectManager.BussinesLayer
{
    [ExportRule(RuleNames.TimeCalculationCalculationValidation)]
    public sealed class TimeCalculationCalculationValidationRule : IOBusinessRuleBase<CalculateTimeInput, CalculateValidateOutput>
    {
        public override CalculateValidateOutput ActionImpl(CalculateTimeInput input)
        {
            var valid = ValidationRuleHelper.AssertList(new[]
                                                        {
                                                            new AssertHelp(input.Speed > 0.059, "TimeCalcSpeedLow"),
                                                            new AssertHelp(input.Speed <= 0.7, "TimeCalcSpeeHight"),
                                                            new AssertHelp(input.Amount > 0, "TimeCalcAmountToLow"),
                                                            new AssertHelp(input.Iterations > 0, "TimeCalcIterationToLow"),
                                                            new AssertHelp(input.PaperFormat.Success, "TimeCalcPaperFormatWrong")
                                                        }, out var msg);

            return new CalculateValidateOutput(valid, msg);
        }
    }
}