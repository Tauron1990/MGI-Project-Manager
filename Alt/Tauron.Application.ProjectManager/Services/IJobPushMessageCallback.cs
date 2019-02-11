using System.ServiceModel;
using Tauron.Application.ProjectManager.Services.Data.Entitys;

namespace Tauron.Application.ProjectManager.Services
{
    public interface IJobPushMessageCallback
    {
        [OperationContract(IsOneWay = true)]
        void Pong();

        [OperationContract(IsOneWay = true)]
        void Close();

        [OperationContract]
        void JobStateTransition(string name, JobStatus status);

        [OperationContract]
        void CurrentJobChanged(string name);
    }
}