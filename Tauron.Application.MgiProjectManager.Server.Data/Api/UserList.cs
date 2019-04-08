using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tauron.Application.MgiProjectManager.Server.Data.Api
{
    [DataContract]
    public class UserList
    {
        [DataMember] public List<AppUser> Items { get; set; } = new List<AppUser>();

        [DataMember] public int Count { get; set; }
    }
}