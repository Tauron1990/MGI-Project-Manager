using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tauron.MgiManager.User.Service.Data.Entitys
{
    public class UserRole
    {
        [Key]
        public Guid Id { get; set; }

        public List<Claim> Claims { get; set; }
    }
}