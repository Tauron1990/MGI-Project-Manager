using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tauron.Application.MgiProjectManager.Server.Data.Entitys
{
    public class OperationEntity
    {
        private List<OperationContextEntity> _context;

        [Key]
        public string OperationId { get; set; }

        public string OperationType { get; set; }

        public bool Compled { get; set; }

        public bool Removed { get; set; }

        public string CurrentOperation { get; set; }

        public string NextOperation { get; set; }

        public DateTime ExpiryDate { get; set; }

        public List<OperationContextEntity> Context
        {
            get => _context ?? (_context = new List<OperationContextEntity>());
            set => _context = value;
        }
    }
}