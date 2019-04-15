using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tauron.MgiProjectManager.Data.Models
{
    [Table("OperationContexts")]
    public class OperationContextEntity
    {
        [Key]
        public string Name { get; set; }

        public string Value { get; set; }
    }
}