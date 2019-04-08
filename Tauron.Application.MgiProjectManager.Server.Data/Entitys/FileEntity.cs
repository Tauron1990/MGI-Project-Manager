using System;
using System.ComponentModel.DataAnnotations;

namespace Tauron.Application.MgiProjectManager.Server.Data.Entitys
{
    public class FileEntity
    {
        [Key]
        public int Id { get; set; }
        public string User { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsRequested { get; set; }

        public string Name { get; set; }

        public DateTime Age { get; set; }

        public string Path { get; set; }
    }
}