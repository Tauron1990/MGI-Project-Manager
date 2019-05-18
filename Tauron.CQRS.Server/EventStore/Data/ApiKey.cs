using System.ComponentModel.DataAnnotations;

namespace Tauron.CQRS.Server.EventStore.Data
{
    public class ApiKey
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Key { get; set; }
    }
}