using System;
using Tauron.CQRS.Services;

namespace Tauron.MgiManager.User.Shared.Events
{
    public class ClaimRemovedFromRoleEvent : BaseEvent
    {
        public override Guid Id { get; set; }

        public string Data { get; set; }

        public Guid RoleId { get; set; }

        public ClaimRemovedFromRoleEvent()
        {
            
        }

        public ClaimRemovedFromRoleEvent(Guid id, string data, Guid roleId)
        {
            Id = id;
            Data = data;
            RoleId = roleId;
        }
    }
}