using System;
using CQRSlite.Events;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Tauron.CQRS.Services;
using Tauron.CQRS.Services.Core;

namespace ServiceManager.CQRS.Logging
{
    [PublicAPI]
    public sealed class LoggingEvent : BaseEvent
    {
        private static Guid NameSpace = new Guid("CBF47E09-F8D1-4A51-B6F9-CD28927A2FAE");

        public override Guid Id { get; set; }

        public string CategoryName { get; set; }
        public LogLevel LogLevel { get; set; }
        public SaveableEventId EventId { get; set; }
        public Exception Exception { get; set; }
        public string Message { get; set; }
        public int Scope { get; set; }

        public LoggingEvent()
        {
            
        }

        public LoggingEvent(string categoryName, LogLevel logLevel, EventId eventId, Exception exception, string message, int scope)
        {
            EventId = new SaveableEventId
            {
                Id = eventId.Id,
                Name = eventId.Name
            };
            CategoryName = categoryName;
            LogLevel = logLevel;
            Exception = exception;
            Message = message;
            Scope = scope;
            Id = IdGenerator.Generator.NewGuid(NameSpace, categoryName);
        }
    }
}