using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;
using MathNet.Numerics;
using Microsoft.EntityFrameworkCore;
using Tauron.Application.Common.BaseLayer;
using Tauron.Application.Common.BaseLayer.BusinessLayer;
using Tauron.Application.Common.BaseLayer.Core;
using Tauron.Application.Common.BaseLayer.Data;
using Tauron.Application.MgiProjectManager.Data.Entitys;
using Tauron.Application.MgiProjectManager.Properties;
using Tauron.Application.MgiProjectManager.ServiceLayer.Dto;

namespace Tauron.Application.MgiProjectManager.BussinesLayer
{
    [ExportRule(RuleNames.TimeCalculationCalculateTime)]
    public sealed class TimeCalculationCalculateTimeRule : IOBusinessRuleBase<CalculateTimeInput, CalculateTimeOutput>
    {
        [InjectRuleFactory]
        private RuleFactory _ruleFactory;

        public override CalculateTimeOutput ActionImpl([NotNull] CalculateTimeInput input)
        {
            var rule      = _ruleFactory.CreateIioBusinessRule<CalculateTimeInput, CalculateValidateOutput>(RuleNames.TimeCalculationCalculationValidation);
            var valOutput = rule.Action(input);
            if (!valOutput.Valid)
                return new CalculateTimeOutput(null, null, null, valOutput.Message, PrecisionMode.InValid);

            var dic = AggregateEntitys(input);

            var setupTime = TimeSpan.FromMinutes(Settings.Default.SetupTime);

            // ReSharper disable PossibleInvalidOperationException

            var iterationTime   = TimeSpan.FromMinutes(Settings.Default.IterationTime * (int) input.Iterations);
            var effectiveAmount = CalculateEffectiveAmount((int) input.Amount, (int) input.Iterations);


            if (dic[PrecisionMode.Perfect].Count > 0)
                return new CalculateTimeOutput(setupTime, iterationTime,
                                               FindPerfectRuntime(dic[PrecisionMode.Perfect], effectiveAmount), "CommonLabelOk", PrecisionMode.Perfect);

            if (dic[PrecisionMode.AmountMismatchPerfect].Count > 0)
                return new CalculateTimeOutput(setupTime, iterationTime,
                                               FindPerfectRuntime(dic[PrecisionMode.AmountMismatchPerfect], effectiveAmount), "CommonLabelOk",
                                               PrecisionMode.AmountMismatchPerfect);

            if (dic[PrecisionMode.NearCorner].Count > 0)
                return new CalculateTimeOutput(setupTime, iterationTime,
                                               FindNearCornerRuntime(dic[PrecisionMode.NearCorner], effectiveAmount, input.Speed.Value),
                                               "CommonLabelOk", PrecisionMode.NearCorner);

            if (dic[PrecisionMode.AmountMismatchNearCorner].Count > 0)
                return new CalculateTimeOutput(setupTime, iterationTime,
                                               FindNearCornerRuntime(dic[PrecisionMode.AmountMismatchNearCorner], effectiveAmount, input.Speed.Value),
                                               "CommonLabelOk", PrecisionMode.AmountMismatchNearCorner);
            // ReSharper restore PossibleInvalidOperationException

            return new CalculateTimeOutput(null, null, null, "TimeCalculationNoDataLabel", PrecisionMode.NoData);
        }

        private int CalculateEffectiveAmount(int amount, int iterations)
        {
            return amount * iterations;
        }

        private TimeSpan FindPerfectRuntime(List<JobRunEntity> entrys, int amount)
        {
            return TimeSpan.FromMinutes(entrys.Average(e => e.NormaizedTime.TotalMinutes) * (amount / 1000d));
        }

        private ImmutableSortedDictionary<double, List<JobRunEntity>> Sort(IEnumerable<JobRunEntity> entry, double speed)
        {
            var sortDic = new Dictionary<double, List<JobRunEntity>>();

            foreach (var clusterEntry in entry)
            {
                var clusterSpeed = clusterEntry.Speed;

                var potential = new List<double>(sortDic.Keys) {speed};
                var added     = false;

                foreach (var key in potential)
                {
                    if (!key.AlmostEqual(clusterSpeed, Settings.Default.PefectDifference)) continue;

                    sortDic[speed].Add(clusterEntry);
                    added = true;
                    break;
                }

                if (added) continue;

                if (!sortDic.ContainsKey(clusterSpeed))
                    sortDic[clusterSpeed] = new List<JobRunEntity> {clusterEntry};
                else
                    sortDic[clusterSpeed].Add(clusterEntry);
            }

            return sortDic.ToImmutableSortedDictionary();
        }

        private TimeSpan FindNearCornerRuntime(IEnumerable<JobRunEntity> entrys, int amount, double speed)
        {
            var points = new List<double>();
            var values = new List<double>();

            foreach (var speedEntry in Sort(entrys, speed))
            {
                points.Add(speedEntry.Key);
                values.Add(speedEntry.Value.Average(ce => ce.NormaizedTime.TotalMinutes));
            }

            return TimeSpan.FromMinutes(Interpolate.Linear(points, values).Interpolate(speed) * (amount / 1000d));
        }

        private Dictionary<PrecisionMode, List<JobRunEntity>> AggregateEntitys(CalculateTimeInput input)
        {
            var cluster = new Dictionary<PrecisionMode, List<JobRunEntity>>
                          {
                              {PrecisionMode.Perfect, new List<JobRunEntity>()},
                              {PrecisionMode.AmountMismatchPerfect, new List<JobRunEntity>()},
                              {PrecisionMode.NearCorner, new List<JobRunEntity>()},
                              {PrecisionMode.AmountMismatchNearCorner, new List<JobRunEntity>()}
                          };

            if (input.PaperFormat.Lenght == null) return cluster;
            if (input.Speed == null) return cluster;
            if (input.Amount == null) return cluster;

            var speed = input.Speed.Value;

            var settings      = Settings.Default;
            var lenght        = input.PaperFormat.Lenght.Value;
            var datetimeTicks = DateTime.Now.Ticks;
            var currentQuatal = CalculateQuatal(DateTime.Now);
            var expire        = settings.EntityExpire;
            var tolerance     = settings.NearCornerDifference;
            var amount        = input.Amount.Value;


            using (RepositoryFactory.Enter())
            {
                var repo = RepositoryFactory.GetRepository<Repository<JobRunEntity, int>>();

                foreach (var entry in repo.Query().AsNoTracking()
                                          .Where(e => e.Length == lenght)
                                          .Where(e => e.Speed.AlmostEqual(speed, tolerance))
                                          .Where(e => e.StartTime.Ticks + expire < datetimeTicks)
                                          .Where(e => CalculateQuatal(e.StartTime) == currentQuatal))
                {
                    if (entry.Speed.AlmostEqual(speed, settings.PefectDifference))
                        if (IsAmoutMismatch(entry.Amount, amount))
                            cluster[PrecisionMode.AmountMismatchPerfect].Add(entry);
                        else
                            cluster[PrecisionMode.Perfect].Add(entry);
                    else if (entry.Speed.AlmostEqual(speed, settings.NearCornerDifference))
                        if (IsAmoutMismatch(entry.Amount, amount))
                            cluster[PrecisionMode.AmountMismatchNearCorner].Add(entry);
                        else
                            cluster[PrecisionMode.NearCorner].Add(entry);
                }
            }

            return cluster;
        }

        private bool IsAmoutMismatch(long set, long ist)
        {
            var tolerance = Settings.Default.AmoutMismatch;

            return ((double) set).AlmostEqualNumbersBetween(ist, tolerance);
        }

        private int CalculateQuatal(DateTime dateTime)
        {
            if (dateTime.Month < 4) return 1;
            if (dateTime.Month < 7) return 2;
            if (dateTime.Month < 10) return 3;
            return 4;
        }
    }
}