using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tauron.MgiProjectManager.Data.Models
{
    [Table("FileBlobs")]
    public class FileBlobEntity
    {
        [Key]
        public int Key { get; set; }

        public byte[] Data { get; set; }
    }
}