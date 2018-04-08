using System;
using System.Runtime.Serialization;

namespace Tauron.Application.ProjectManager.Services.DTO
{
    [DataContract]
    public class GenericServiceFault
    {
        [DataMember]
        public Type ErrorType { get; }
        [DataMember]
        public string Reason { get; }

        public GenericServiceFault(Type errorType, string reason)
        {
            ErrorType = errorType;
            Reason = reason;
        }
    }
}