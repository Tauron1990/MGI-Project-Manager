using System;
using System.Runtime.Serialization;

namespace Tauron.Application.ProjectManager.Services.DTO
{
    [DataContract]
    public class GenericServiceFault
    {
        public GenericServiceFault(Type errorType, string reason)
        {
            ErrorType = errorType;
            Reason    = reason;
        }

        [DataMember]
        public Type ErrorType { get; }

        [DataMember]
        public string Reason { get; }
    }
}