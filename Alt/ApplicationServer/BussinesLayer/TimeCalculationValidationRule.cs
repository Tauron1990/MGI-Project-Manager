using System;
using SQLitePCL;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.Core;
using Tauron.Application.ProjectManager.ApplicationServer.Core;
using Tauron.Application.ProjectManager.Resources;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.ApplicationServer.BussinesLayer
{
    [ExportRule(RuleNames.TimeCalculationValidation)]
    public sealed class TimeCalculationValidationRule : IOBusinessRuleBase<ValidationInput, ValidationOutput>
    {
        public override ValidationOutput ActionImpl(ValidationInput input)
        {
            var settings = SettingsManager.Default;

            if (!ValidationRuleHelper.AssertList(new[]
                                                 {
                                                     new AssertHelp(input.Amount != null, "TimeCalcAmountNull"),
                                                     new AssertHelp(input.Iteration != null, "TimeCalcIterationNull"),
                                                     new AssertHelp(input.Amount > 0, "TimeCalcAmountToLow"),
                                                     new AssertHelp(input.Iteration > 0, "TimeCalcIterationToLow"),
                                                     new AssertHelp(input.Format.Success, "TimeCalcPaperFormatWrong"),
                                                     new AssertHelp(input.Format.Lenght > 29 && input.Format.Width > 19, "TimeCalcFormatToSmall"),
                                                     new AssertHelp(input.Format.Lenght < 101 && input.Format.Width < 52, "TimeCalcFormatToLarge"),
                                                     new AssertHelp(input.Speed != null, "TimeCalcSpeedNull"),
                                                     new AssertHelp(input.Speed > 0.059, "TimeCalcSpeedLow"),
                                                     new AssertHelp(input.Speed <= 0.7, "TimeCalcSpeedHight"),
                                                     new AssertHelp(input.RunTime > TimeSpan.FromSeconds(1), "TimeCalcTimeLow")
                                                 }, out var formattedValue)) return new ValidationOutput(formattedValue, null);

            // ReSharper disable PossibleInvalidOperationException
            var iterationTime = input.Iteration.Value / (double) settings.IterationTime;

            var realRunTime = input.RunTime - TimeSpan.FromMinutes(iterationTime);

            var runtime = realRunTime.TotalMinutes / input.Iteration.Value / input.Amount.Value * 1000d;

            TimeSpan? normalizedTime = TimeSpan.FromMinutes(runtime);

            formattedValue = $"{ServerMessages.LabelTimeCalcNormalizedTime}: {normalizedTime.Value.Hours}:{normalizedTime.Value.Minutes} ({ServerMessages.LabelTimeCalcPerThousand})";

            // ReSharper restore PossibleInvalidOperationException

            return new ValidationOutput(formattedValue, normalizedTime);
        }
    }
}