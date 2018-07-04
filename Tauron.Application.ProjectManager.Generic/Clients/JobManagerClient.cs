using System.ServiceModel;
using System.ServiceModel.Channels;
using Tauron.Application.ProjectManager.Services;
using Tauron.Application.ProjectManager.Services.Data.Entitys;
using Tauron.Application.ProjectManager.Services.DTO;

namespace Tauron.Application.ProjectManager.Generic.Clients
{
    public class JobManagerClient : ClientHelperBase<IJobManager>, IJobManager
    {
        public JobManagerClient(Binding binding, EndpointAddress adress) : base(binding, adress)
        {
        }

        public JobItemDto[] GetActiveJobs() => Secure(() => Channel.GetActiveJobs());

        public bool InsertJob(JobItemDto jobItem) => Secure(() => Channel.InsertJob(jobItem));

        public string ValidateJob(JobItemDto jobItem) => Secure(() => Channel.ValidateJob(jobItem));

        public void MarkImportent(JobItemDto jobItem) => Secure(() => Channel.MarkImportent(jobItem));

        public bool StateTransition(JobStatus status) => Secure(() => Channel.StateTransition(status));

        public bool StartJob(string name) => Secure(() => Channel.StartJob(name));

        public JobItemDto GetCurrentJob() => Secure(() => Channel.GetCurrentJob());
    }
}