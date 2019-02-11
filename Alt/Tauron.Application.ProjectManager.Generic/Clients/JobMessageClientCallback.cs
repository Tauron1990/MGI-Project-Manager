using System;
using System.Threading.Tasks;
using Tauron.Application.ProjectManager.Services;
using Tauron.Application.ProjectManager.Services.Data.Entitys;

namespace Tauron.Application.ProjectManager.Generic.Clients
{
    public sealed class JobMessageClientCallback : CallbackBase<JobPushMessageClient>, IJobPushMessageCallback
    {
        public void Pong() => Client.TimeManager.ConnectionAlive();

        public void Close() => Task.Run((Action)Client.Close);

        public void JobStateTransition(string name, JobStatus status) => Client.OnJobStateTransition(name, status);

        public void CurrentJobChanged(string name) => Client.OnCurrentJobChanged(name);
    }
}