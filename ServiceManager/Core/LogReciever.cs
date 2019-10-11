using System.Threading.Tasks;
using CQRSlite.Events;
using ServiceManager.CQRS.Logging;
using Tauron.CQRS.Services.Extensions;

namespace ServiceManager.Core
{
    [CQRSHandler]
    public class LogReciever : IEventHandler<LoggingEvent>
    {
        private readonly LogEntries _logEntries;

        public LogReciever(LogEntries logEntries) 
            => _logEntries = logEntries;

        public Task Handle(LoggingEvent message)
        {
            _logEntries.AddLog(message.ServiceName, message.Message);

            return Task.CompletedTask;
        }
    }
}