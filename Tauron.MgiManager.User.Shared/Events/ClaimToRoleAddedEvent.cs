using System;
using Tauron.CQRS.Services;

namespace Tauron.MgiManager.User.Shared.Events
{
    public class ClaimToRoleAddedEvent : BaseEvent
    {
        public override  Guid Id { get; set; }

        public string Data { get; set; }

        public Guid RoleId { get; set; }

        public ClaimToRoleAddedEvent()
        {
            
        }

        public ClaimToRoleAddedEvent(Guid aggregateId, string data, Guid roleId)
        {
            Id = aggregateId;
            Data = data;
            RoleId = roleId;
        }
    }
}