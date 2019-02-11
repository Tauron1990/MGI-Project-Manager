using System;
using System.Runtime.Serialization;
using Tauron.Application.ProjectManager.Services.Data.Entitys;

namespace Tauron.Application.ProjectManager.Services.DTO
{
    [DataContract]
    public class JobItemDto : IEquatable<JobItemDto>
    {
        [DataMember]
        public DateTime TargetDate { get; set; }

        [DataMember]
        public string LongName { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public JobStatus Status { get; set; }

        [DataMember]
        public bool Importent { get; set; }

        public static JobItemDto FromEntity(JobEntity entity)
        {
            if (entity == null)
                return null;

            return new JobItemDto
                   {
                       Importent  = entity.Importent,
                       LongName   = entity.LongName,
                       Name       = entity.Id,
                       Status     = entity.Status,
                       TargetDate = entity.TargetDate
                   };
        }

        public bool Equals(JobItemDto other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return TargetDate.Equals(other.TargetDate) && string.Equals(LongName, other.LongName) && string.Equals(Name, other.Name) && Status == other.Status && Importent == other.Importent;
        }
    }
}