using System;
using System.ComponentModel.DataAnnotations;

namespace Tauron.MgiManager.User.Service.Data.Entitys
{
    public sealed class Claim
    {
        [Key]
        public int Id { get; set; }

        public Guid ClaimId { get; set; }

        public string Data { get; set; }
    }
}