using System;
using System.ComponentModel.DataAnnotations;

namespace Tauron.Application.MgiProjectManager.Server.Data.Entitys
{
    public sealed class TimedTaskEntity
    {
        [Key]
        public string Name { get; set; }

        public DateTime LastRun { get; set; }
    }
}