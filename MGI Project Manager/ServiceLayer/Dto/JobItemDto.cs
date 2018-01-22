using System;
using Tauron.Application.MgiProjectManager.Data;

namespace Tauron.Application.MgiProjectManager.ServiceLayer.Dto
{
    public class JobItemDto
    {
        public DateTime TargetDate { get; set; }

        public string LongName { get; set; }

        public string Name { get; set; }

        public JobStatus Status { get; set; }

        public bool Importent { get; set; }
    }
}