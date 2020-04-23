using System.Windows;
using System.Windows.Media;
using Tauron.Application.ToolUI.Core;
using Tauron.Application.ToolUI.ViewModels;
using Tauron.Application.Wpf;

namespace Tauron.Application.ToolUI.Views
{
    /// <summary>
    /// Interaktionslogik für LogEntryView.xaml
    /// </summary>
    [Control(typeof(LogEntryViewModel))]
    public partial class LogEntryView
    {
        public LogEntryView(LogEntryViewModel model)
            : base(model)
        {
            InitializeComponent();
            model.Events = SerilogViewer.LogEntries;
            Background = Brushes.Transparent;
        }

        private void ScrollUp(object sender, RoutedEventArgs e) 
            => SerilogViewer.ScrollToFirst();

        private void ScrollDown(object sender, RoutedEventArgs e) 
            => SerilogViewer.ScrollToLast();

        private void Clear(object sender, RoutedEventArgs e) 
            => SerilogViewer.Clear();
    }
}
