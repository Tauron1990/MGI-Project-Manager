using System;
using System.ComponentModel.DataAnnotations;

namespace Tauron.MgiProjectManager.Data.Models
{
    public class FileBlobInfoEntity
    {
        public long Size { get; set; }

        [Key]
        public string FileName { get; set; }

        public DateTime CreationTime { get; set; }
    }
}