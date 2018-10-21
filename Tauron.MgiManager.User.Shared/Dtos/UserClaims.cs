using System;
using System.Collections.Generic;

namespace Tauron.MgiManager.User.Shared.Dtos
{
    public class UserClaims
    {
        public List<UserClaim> Claims { get; set; } = new List<UserClaim>();

        public Guid Id { get; set; }

        public UserClaims()
        {
            
        }

        public UserClaims(Guid id)
        {
            Id = id;
        }
    }
}