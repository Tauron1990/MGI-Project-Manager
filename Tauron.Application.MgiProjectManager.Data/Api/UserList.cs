using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tauron.Application.MgiProjectManager.Data.Api
{
    [DataContract]
    public class UserList
    {
        [DataMember]
        public List<User> Items { get; set; } = new List<User>();

        [DataMember]
        public int Count { get; set; }
    }
}