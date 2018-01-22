using System;
using System.Collections.Generic;
using System.Linq;
using Tauron.Application.Common.BaseLayer.BusinessLayer;
using Tauron.Application.Ioc;
using Tauron.Application.MgiProjectManager.BussinesLayer;
using Tauron.Application.MgiProjectManager.ServiceLayer.Dto;

namespace Tauron.Application.MgiProjectManager.ServiceLayer.Impl
{
    [Export(typeof(IJobManager))]
    public sealed class JobManager : IJobManager
    {
        [Inject]
        private RuleFactory _ruleFactory;

        public IEnumerable<JobItemDto> GetActiveJobs()
        {
            return ExecutionHelper.ExecuteRule(_ruleFactory.CreateOBussinesRule<IEnumerable<JobItemDto>>(RuleNames.GetActiveJobsRule),
                                               Enumerable.Empty<JobItemDto>(), nameof(IJobManager));
        }

        public bool InsertJob(JobItemDto jobItem)
        {
            return ExecutionHelper.ExecuteRule(_ruleFactory.CreateComposite<JobItemDto, bool>(RuleNames.ValidateJob, RuleNames.InsertJob),
                                               jobItem, false, nameof(IJobManager));
        }

        public string ValidateJob(JobItemDto dto)
        {
            var rule = _ruleFactory.CreateIiBusinessRule<JobItemDto>(RuleNames.ValidateJob);
            rule.Action(dto);

            if (!rule.Error) return null;

            var temp = rule.Errors.First();
            if (temp is Exception ex) return ex.Message;
            return temp.ToString();
        }

        public void MarkImportent(JobItemDto jobItem)
        {
            ExecutionHelper.ExecuteRule(_ruleFactory.CreateIiBusinessRule<JobItemDto>(RuleNames.MarkImportent), jobItem, nameof(IJobManager));
        }
    }
}