using System.Runtime.Serialization;

namespace Tauron.Application.MgiProjectManager.Data.Api
{
    [DataContract]
    public sealed class User
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Role { get; set; }
    }
}