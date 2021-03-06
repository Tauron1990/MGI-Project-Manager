﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tauron.MgiManager.User.Service.Data.Entitys
{
    public class UserRole
    {
        [Key]
        public Guid Id { get; set; }

        public List<Claim> Claims { get; set; }

        public UserRole()
        {
            
        }

        public UserRole(Guid id, List<Claim> claims)
        {
            Id = id;
            Claims = claims;
        }
    }
}