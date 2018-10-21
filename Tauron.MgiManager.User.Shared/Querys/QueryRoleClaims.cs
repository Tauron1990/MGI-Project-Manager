using System;
using CQRSlite.Queries;
using Tauron.MgiManager.User.Shared.Dtos;

namespace Tauron.MgiManager.User.Shared.Querys
{
    public sealed class QueryRoleClaims : IQuery<UserClaims>
    {
        public Guid RoleId { get; set; }

        public QueryRoleClaims()
        {
            
        }

        public QueryRoleClaims(Guid roleId) => RoleId = roleId;
    }
}