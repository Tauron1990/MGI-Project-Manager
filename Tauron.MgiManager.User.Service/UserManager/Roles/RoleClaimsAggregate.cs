using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Tauron.CQRS.Services;
using Tauron.MgiManager.User.Shared.Events;

namespace Tauron.MgiManager.User.Service.UserManager.Roles
{
    public sealed class RoleClaimsAggregate : CoreAggregateRoot
    {
        public List<string> Claims
        {
            get
            {
                //TODO ID Speichern
                ClaimID Specihern
                var list = GetValue<List<string>>();
                if (list != null) return list;

                list = new List<string>();
                SetValue(list);
                return list;

            }
        }

        [UsedImplicitly]
        private void Apply(ClaimToRoleAddedEvent revent)
        {
            if(!Claims.Contains(revent.Data))
                Claims.Add(revent.Data);
        }

        public RoleClaimsAggregate(Guid id) 
            => Id = id;

        public RoleClaimsAggregate()
        {
            
        }
    }
}