using System;
using System.ComponentModel.DataAnnotations;

namespace Tauron.MgiProjectManager.Data.Models
{
    public sealed class TimedTaskEntity
    {
        [Key]
        public string Name { get; set; }

        public DateTime LastRun { get; set; }
    }
}