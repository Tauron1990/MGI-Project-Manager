using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using Serilog.Events;

namespace Tauron.MgiProjectManager.Data.Models
{
    public class LoggingEventEntity
    {
        [Key]
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }

        public LogEventLevel Level { get; set; }
        
        public string Properties;

        public string Message { get; set; }

        public string Exception { get; set; }

        public LoggingEventEntity(LogEvent logEvent)
        {
            Exception = logEvent.Exception?.ToStringDemystified() ?? string.Empty;
            Message = logEvent.RenderMessage();
            Properties = string.Join(", ", logEvent.Properties.Select(p => $"{{{p.Key}, {p.Value.ToString()}}}"));
            Level = logEvent.Level;
            Timestamp = new DateTime(logEvent.Timestamp.Ticks);
        }

        public LoggingEventEntity()
        {
            
        }
    }
}