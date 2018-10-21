using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Tauron.CQRS.Services;
using Tauron.MgiManager.User.Shared.Events;

namespace Tauron.MgiManager.User.Service.UserManager.Roles
{
    public sealed class RoleClaimsAggregate : CoreAggregateRoot
    {
        public Dictionary<Guid, string> Claims
        {
            get
            {
                var list = GetValue<Dictionary<Guid, string>>();
                if (list != null) return list;

                list = new Dictionary<Guid, string>();
                SetValue(list);
                return list;

            }
        }

        [UsedImplicitly]
        private void Apply(ClaimToRoleAddedEvent revent) 
            => Claims[revent.ClaimId] = revent.Data;

        public RoleClaimsAggregate(Guid id) 
            => Id = id;

        public RoleClaimsAggregate()
        {
            
        }
    }
}