using System.Runtime.Serialization;

namespace Tauron.Application.ProjectManager.Services.DTO
{
    [DataContract]
    public class LogInFault
    {
        public LogInFault(string reason)
        {
            Reason = reason;
        }

        [DataMember]
        public string Reason { get; }
    }
}