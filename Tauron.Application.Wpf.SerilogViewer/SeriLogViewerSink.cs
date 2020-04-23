using System;
using Serilog.Core;
using Serilog.Events;

namespace Tauron.Application.Wpf.SerilogViewer
{
    public sealed class SeriLogViewerSink : ILogEventSink
    {
        internal static SeriLogViewerSink? CurrentSink { get; private set; }

        public event Action<LogEvent> LogReceived;

        public LimitedList<LogEvent> Logs { get; } = new LimitedList<LogEvent>(100);

        public SeriLogViewerSink()
        {
            CurrentSink = this;
        }

        public void Emit(LogEvent logEvent)
        {
            Logs.Add(logEvent);
            LogReceived?.Invoke(logEvent);
        }
    }
}
