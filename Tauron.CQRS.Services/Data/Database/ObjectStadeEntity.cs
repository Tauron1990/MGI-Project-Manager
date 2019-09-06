using System.ComponentModel.DataAnnotations;

namespace Tauron.CQRS.Services.Data.Database
{
    public class ObjectStadeEntity
    {
        [Key]
        public string Identifer { get; set; }

        public string OriginType { get; set; }

        public string Data { get; set; }
    }
}