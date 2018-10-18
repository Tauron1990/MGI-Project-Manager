using System;
using System.ComponentModel.DataAnnotations;

namespace Tauron.MgiManager.User.Service.Data.Entitys
{
    public sealed class ApplicationUser
    {
        public string Salt { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        [Key]
        public Guid Id { get; set; }
    }
}