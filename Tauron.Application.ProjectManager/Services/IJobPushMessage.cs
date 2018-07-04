using System.ServiceModel;

namespace Tauron.Application.ProjectManager.Services
{
    [ServiceContract(CallbackContract = typeof(IJobPushMessageCallback), Namespace = "MGI-Proleckt-Server", SessionMode = SessionMode.Required)]
    public interface IJobPushMessage
    {
        [OperationContract(IsOneWay = true)]
        void Ping();

        [OperationContract(IsInitiating = true)]
        void Register();

        [OperationContract(IsTerminating = true)]
        void UnRegister();
    }
}