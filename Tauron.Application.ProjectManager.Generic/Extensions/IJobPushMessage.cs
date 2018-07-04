using System;
using Tauron.Application.ProjectManager.Services.Data.Entitys;

namespace Tauron.Application.ProjectManager.Generic.Extensions
{
    public interface IJobPushMessageExtension
    {
        event Action ConnectionLost;
        event Action ConnectionEstablisht;
        event Action<string, JobStatus> JobStateTransition;

        event Action<string> CurrentJobChanged;

        void StartListening();
        void StopListening();
    }
}