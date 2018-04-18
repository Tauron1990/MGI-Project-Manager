using System.Runtime.Serialization;

namespace Tauron.Application.ProjectManager.Services.DTO
{
    [DataContract]
    public class GenericServiceResult
    {
        public GenericServiceResult(bool succededSuccessful, string reason)
        {
            SuccededSuccessful = succededSuccessful;
            Reason             = reason;
        }

        [DataMember]
        public bool SuccededSuccessful { get; }

        [DataMember]
        public string Reason { get; }
    }
}