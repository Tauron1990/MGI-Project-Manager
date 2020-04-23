using System;
using System.Globalization;
using System.Windows.Media;
using Serilog.Events;

namespace Tauron.Application.Wpf.SerilogViewer
{
    public class LogEventViewModel
    {
        public SerilogEvent Info { get; }

        public LogEventViewModel(SerilogEvent info)
        {
            Info = info;
            var logEventInfo = info.EventInfo;

            var msg = logEventInfo.RenderMessage();

            ToolTip = msg;
            Level = logEventInfo.Level.ToString();
            FormattedMessage = msg;
            Exception = logEventInfo.Exception;
            LoggerName = logEventInfo.Properties.TryGetValue("SourceContext", out var value) ? value.ToString() : "Unbekannt";
            Time = logEventInfo.Timestamp.ToString(CultureInfo.InvariantCulture);

            SetupColors(logEventInfo);
        }


        public string Time { get; }
        public string LoggerName { get; }
        public string Level { get; }
        public string FormattedMessage { get; }
        public Exception Exception { get; }
        public string ToolTip { get; }
        public SolidColorBrush? Background { get; private set; }
        public SolidColorBrush? Foreground { get; private set; }
        public SolidColorBrush? BackgroundMouseOver { get; private set; }
        public SolidColorBrush? ForegroundMouseOver { get; private set; }

        private void SetupColors(LogEvent logEventInfo)
        {
            if (logEventInfo.Level == LogEventLevel.Warning)
            {
                Background = Brushes.DarkOrange;
                BackgroundMouseOver = Brushes.DarkGoldenrod;
            }
            else if (logEventInfo.Level == LogEventLevel.Error || logEventInfo.Level == LogEventLevel.Fatal)
            {
                Background = Brushes.DarkRed;
                BackgroundMouseOver = Brushes.DarkRed;
            }
            else
            {
                Background = Brushes.Black;
                BackgroundMouseOver = Brushes.DarkGray;
            }
            Foreground = Brushes.White;
            ForegroundMouseOver = Brushes.White;
        }
    }
}