using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Tauron.Application.Common.BaseLayer.Data;

namespace Tauron.Application.ProjectManager.Services.Data.Entitys
{
    [Serializable]
    [PublicAPI]
    public class JobEntity : GenericBaseEntity<string>
    {
        //public int JobRunId { get; set; }

        public List<JobRunEntity> JobRuns { get; set; }

        public DateTime TargetDate { get; set; }

        public string LongName { get; set; }

        public JobStatus Status { get; set; }

        public bool Importent { get; set; }

        public bool IsActive { get; set; }
    }
}