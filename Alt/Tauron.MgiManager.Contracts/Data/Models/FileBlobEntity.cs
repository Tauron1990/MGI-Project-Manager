using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tauron.MgiProjectManager.Data.Models
{
    public class FileBlobEntity
    {
        [Key]
        public string Key { get; set; }

        public byte[] Data { get; set; }
    }
}