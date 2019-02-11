using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Tauron.Application.Common.BaseLayer.BusinessLayer;
using Tauron.Application.Ioc;
using Tauron.Application.ProjectManager.ApplicationServer.BussinesLayer;
using Tauron.Application.ProjectManager.ApplicationServer.Data;
using Tauron.Application.ProjectManager.Services.DTO;
using TestHelperLib;
using Xunit;

namespace ApplicationServer.Tests.BussinesLayer
{
    public class TimeCalculationCalculationValidationRuleTest : IDisposable
    {
        private readonly IContainer _container;
        private readonly InMemoryDatabaseRoot _inMemoryDatabaseRoot = new InMemoryDatabaseRoot();

        public TimeCalculationCalculationValidationRuleTest()
        {
            SetupApplication.AddSetupAction(() =>
                DatabaseImpl.UpdateSchema(o => o.UseInMemoryDatabase("test", _inMemoryDatabaseRoot)));
            SetupApplication.AddTypes(typeof(MainDatabase));
            _container = SetupApplication.Setup(true, typeof(AddSetupItemRule).Assembly);
        }

        public void Dispose() => SetupApplication.Free();

        [Fact]
        public void Test()
        {
            var rule = _container.GetIoBusinessRule<CalculateTimeInput, CalculateValidateOutput>(RuleNames
                .TimeCalculationCalculationValidation);
            var defaultformat = new PaperFormat(32, 46);

            var valid = CreateTest(new PaperFormat("32 cm x 46 cm"));
            var valid2 = CreateTest(new PaperFormat("32cm x 46cm"));

            Run(rule, new (CalculateTimeInput Input, bool IsValid, string Result)[]
            {
                (valid, true, null),
                (valid2, true, null),
                (CreateTest(new PaperFormat("ABC")), false, "TimeCalcPaperFormatWrong")
            });

            CheckSpeed(rule, defaultformat);
            CheckAmount(rule, defaultformat);
            CheckIteration(rule, defaultformat);
        }

        private void CheckSpeed(IIOBusinessRule<CalculateTimeInput, CalculateValidateOutput> rule, PaperFormat def)
        {
            Run(rule, new (CalculateTimeInput Input, bool IsValid, string Result)[]
            {
                (CreateTest(def, 0.1), false,"TimeCalcSpeedLow"),
                (CreateTest(def,1), false, "TimeCalcSpeeHight")
            });
        }
        
        private void CheckAmount(IIOBusinessRule<CalculateTimeInput, CalculateValidateOutput> rule, PaperFormat def)
        {
            Run(rule, new (CalculateTimeInput Input, bool IsValid, string Result)[]
            {
                (CreateTest(def, amount:0), false, "TimeCalcAmountToLow"),
                (CreateTest(def, amount:-1), false, "TimeCalcAmountToLow")
            });
        }

        private void CheckIteration(IIOBusinessRule<CalculateTimeInput, CalculateValidateOutput> rule, PaperFormat def)
        {
            Run(rule, new (CalculateTimeInput Input, bool IsValid, string Result)[]
            {
                (CreateTest(def, iterations:0), false, "TimeCalcIterationToLow"),
                (CreateTest(def, iterations:-1), false, "TimeCalcIterationToLow")
            });
        }

        private void Run(IIOBusinessRule<CalculateTimeInput, CalculateValidateOutput> rule,
            IEnumerable<(CalculateTimeInput Input, bool IsValid, string Result)> testEnum)
        {
            foreach (var tuple in testEnum)
            {
                var output = rule.Action(tuple.Input);
                Assert.Equal(tuple.IsValid, output.Valid);
                Assert.Equal(tuple.Result, output.Message);
            }
        }

        private CalculateTimeInput CreateTest(PaperFormat paperFormat, double? speed = 0.55, long? amount = 1000, long? iterations = 1) => new CalculateTimeInput("BM18_00001", iterations, paperFormat, amount, speed);
    }
}