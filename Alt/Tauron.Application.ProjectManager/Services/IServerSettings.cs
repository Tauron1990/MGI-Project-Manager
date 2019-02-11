using System.ServiceModel;

namespace Tauron.Application.ProjectManager.Services
{
    [ServiceContract]
    public interface IServerSettings
    {
        [OperationContract]
        int GetIterationTime();
        [OperationContract]
        void SetIterationTime(int value);

        [OperationContract]
        int GetSetupTime();
        [OperationContract]
        void SetSetupTime(int value);

        [OperationContract]
        double GetPefectDifference();
        [OperationContract]
        void SetPefectDifference(double value);

        [OperationContract]
        double GetNearCornerDifference();
        [OperationContract]
        void SetNearCornerDifference(double value);

        [OperationContract]
        long GetEntityExpire();
        [OperationContract]
        void SetEntityExpire(long value);

        [OperationContract]
        int GetAmoutMismatch();
        [OperationContract]
        void SetAmoutMismatch(int value);
    }
}