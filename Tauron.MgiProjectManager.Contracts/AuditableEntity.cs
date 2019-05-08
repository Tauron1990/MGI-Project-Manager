// =============================
// Email: info@ebenmonney.com
// www.ebenmonney.com/templates
// =============================

using System;

namespace Tauron.MgiProjectManager
{
    public class AuditableEntity : IAuditableEntity
    {
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
