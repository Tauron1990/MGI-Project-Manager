using System.ComponentModel.DataAnnotations;

namespace Tauron.CQRS.Server.EventStore.Data
{
    public class ObjectStadeEntity
    {
        [Key]
        public string Identifer { get; set; }

        public string Data { get; set; }
    }
}