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
            Background = Brushes.Transparent;
        }
    }
}
