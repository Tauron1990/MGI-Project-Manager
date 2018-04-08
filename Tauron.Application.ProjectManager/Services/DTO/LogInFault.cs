using System.Runtime.Serialization;

namespace Tauron.Application.ProjectManager.Services.DTO
{
    [DataContract]
    public class LogInFault
    {
        [DataMember]
        public string Reason { get; }

        public LogInFault(string reason)
        {
            Reason = reason;
        }
    }
}