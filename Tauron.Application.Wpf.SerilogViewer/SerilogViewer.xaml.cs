using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Catel.Collections;
using JetBrains.Annotations;
using Serilog.Events;

namespace Tauron.Application.Wpf.SerilogViewer
{
    /// <summary>
    /// Interaktionslogik für SerilogViewer.xaml
    /// </summary>
    [PublicAPI]
    public partial class SerilogViewer
    {
        public ListView LogView => logView;
        public event EventHandler ItemAdded = delegate { };
        public ObservableCollection<LogEventViewModel> LogEntries { get; private set; }
        public bool IsTargetConfigured { get; }

        [Description("Weite der Zeit Spalte in Pixel"), Category("Data")]
        [TypeConverter(typeof(LengthConverter))]
        public double TimeWidth { get; set; } = 120;

        [Description("Weite der Logger Spalte in Pixel oder auto wen nicht angegeben"), Category("Data")]
        [TypeConverter(typeof(LengthConverter))]
        public double LoggerNameWidth { get; set; } = 50;

        [Description("Weite der Level Spalte in Pixel"), Category("Data")]
        [TypeConverter(typeof(LengthConverter))]
        public double LevelWidth { get; set; } = 50;

        [Description("Weite der Nachricht Spalte In Pixel"), Category("Data")]
        [TypeConverter(typeof(LengthConverter))]
        public double MessageWidth { get; set; } = 200;

        [Description("Weite der Exception Spalte In Pixel"), Category("Data")]
        [TypeConverter(typeof(LengthConverter))]
        public double ExceptionWidth { get; set; } = 75;

        [Description("Die maximale anzahl an Zeilen. Der Älteste eintrag wird gelöscht. Auf 0 Setzen für Unbegrenzte Einträge."), Category("Data")]
        [TypeConverter(typeof(Int32Converter))]
        public int MaxRowCount { get; set; } = 200;

        [Description("Automatisch zum letzten eintrag in der Ansicht Scrollen. Standart wert ist An."), Category("Data")]
        [TypeConverter(typeof(BooleanConverter))]
        public bool AutoScrollToLast { get; set; } = true;

        public SerilogViewer()
        {
            IsTargetConfigured = false;
            LogEntries = new ObservableCollection<LogEventViewModel>();

            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this)) return;
            if (SeriLogViewerSink.CurrentSink == null) return;
            IsTargetConfigured = true;

            LogEntries.AddRange(SeriLogViewerSink.CurrentSink.Logs.Select(e => new LogEventViewModel(e)));
            SeriLogViewerSink.CurrentSink.LogReceived += LogReceived;
        }

        protected void LogReceived(LogEvent log)
        {
            LogEventViewModel vm = new LogEventViewModel(log);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (MaxRowCount > 0 && LogEntries.Count >= MaxRowCount)
                    LogEntries.RemoveAt(0);
                LogEntries.Add(vm);
                if (AutoScrollToLast) ScrollToLast();
                ItemAdded(this, (SerilogEvent)log);
            }));
        }
        public void Clear()
        {
            LogEntries.Clear();
        }

        public void ScrollToFirst()
        {
            if (LogView.Items.Count <= 0) return;
            LogView.SelectedIndex = 0;
            ScrollToItem(LogView.SelectedItem);
        }
        public void ScrollToLast()
        {
            if (LogView.Items.Count <= 0) return;
            LogView.SelectedIndex = LogView.Items.Count - 1;
            ScrollToItem(LogView.SelectedItem);
        }

        private void ScrollToItem(object item)
        {
            LogView.ScrollIntoView(item);
        }

        private void SerilogViewer_OnLoaded(object sender, RoutedEventArgs e)
        {
            if(AutoScrollToLast)
                ScrollToLast();
        }
    }
}
