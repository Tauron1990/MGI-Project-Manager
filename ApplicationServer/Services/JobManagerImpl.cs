using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Tauron.Application.ProjectManager.ApplicationServer.BussinesLayer;
using Tauron.Application.ProjectManager.ApplicationServer.Core;
using Tauron.Application.ProjectManager.Services;
using Tauron.Application.ProjectManager.Services.Data.Entitys;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.ApplicationServer.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class JobManagerImpl : ServiceBase, IJobManager
    {
        public JobManagerImpl() => AllowedRights = UserRights.Operater;

        public JobItemDto[] GetActiveJobs() => ExecuteRule(RuleFactory.CreateOBussinesRule<IEnumerable<JobItemDto>>(RuleNames.GetActiveJobsRule),
                                                           Enumerable.Empty<JobItemDto>(), nameof(IJobManager), rights:UserRights.Manager).ToArray();

        public bool InsertJob(JobItemDto jobItem) => ExecuteRule(RuleFactory.CreateComposite<JobItemDto, bool>(RuleNames.ValidateJob, RuleNames.InsertJob),
                                                                 jobItem, false, nameof(IJobManager));

        public string ValidateJob(JobItemDto jobItem)
        {
            return Secure(() =>
                   {
                       var rule = RuleFactory.CreateIiBusinessRule<JobItemDto>(RuleNames.ValidateJob);
                       rule.Action(jobItem);

                       if (!rule.Error) return null;

                       var temp = rule.Errors.First();
                       if (temp is Exception ex) return ex.Message;
                       return temp.ToString();
                   });
        }

        public void MarkImportent(JobItemDto jobItem) => ExecuteRule(RuleFactory.CreateIiBusinessRule<JobItemDto>(RuleNames.MarkImportent), jobItem, nameof(IJobManager), rights:UserRights.Manager);

        public bool StateTransition(string name, JobStatus status) => Secure(() => CurrentJobManager.SetStatus(name, status));

        public bool SeCurrentJob(string name) => Secure(() =>  CurrentJobManager.SetCurrentJob(name));

        public JobItemDto GetCurrentJob() => Secure(() => JobItemDto.FromEntity(CurrentJobManager.CurrentJob), UserRights.Manager);
    }
}