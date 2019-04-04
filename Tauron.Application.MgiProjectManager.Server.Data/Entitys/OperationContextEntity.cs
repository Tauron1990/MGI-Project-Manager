using System.ComponentModel.DataAnnotations;

namespace Tauron.Application.MgiProjectManager.Server.Data.Entitys
{
    public class OperationContextEntity
    {
        [Key]
        public string Name { get; set; }

        public string Value { get; set; }
    }
}