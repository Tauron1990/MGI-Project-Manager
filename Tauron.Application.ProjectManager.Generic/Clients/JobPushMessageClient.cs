using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Tauron.Application.ProjectManager.Generic.Core;
using Tauron.Application.ProjectManager.Generic.Extensions;
using Tauron.Application.ProjectManager.Services;
using Tauron.Application.ProjectManager.Services.Data.Entitys;

namespace Tauron.Application.ProjectManager.Generic.Clients
{
    public sealed class JobPushMessageClient : ClientHelperBase<IJobPushMessage>, IJobPushMessageExtension
    {
        public TimeManager TimeManager { get; private set; }

        public JobPushMessageClient(JobMessageClientCallback context, Binding binding, EndpointAddress adress) 
            : base(context, binding, adress) => context.SetClient(this);

        public void Register() => Secure(() => Channel.Register());

        protected override void InnerClose(Action close)
        {
            try
            {
                Secure(() => Channel.UnRegister());
            }
            catch(ClientException)
            {
            }
            base.InnerClose(close);
        }

        protected override void InnerOpen(Action open)
        {
            if(Channel == null) 
                StartListening();
            else
                base.InnerOpen(open);
        }

        public event Action ConnectionLost;
        public event Action ConnectionEstablisht;
        public event Action<string, JobStatus> JobStateTransition;
        public event Action<string> CurrentJobChanged;

        public void StartListening()
        {
            TimeManager = new TimeManager(this);
            TimeManager.StartListening();
        }

        public void StopListening()
        {
            TimeManager.StopListening();
            TimeManager.Dispose();
            TimeManager = null;
        }

        

        internal void OnConnectionLost() => ConnectionLost?.Invoke();

        internal void OnConnectionEstablisht() => ConnectionEstablisht?.Invoke();

        internal void Ping() => Channel.Ping();

        internal void OnJobStateTransition(string arg1, JobStatus arg2) => JobStateTransition?.Invoke(arg1, arg2);

        internal void OnCurrentJobChanged(string obj) => CurrentJobChanged?.Invoke(obj);
    }
}