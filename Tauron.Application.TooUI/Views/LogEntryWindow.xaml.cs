using System.Windows;
using Tauron.Application.ToolUI.Core;

namespace Tauron.Application.ToolUI.Views
{
    /// <summary>
    /// Interaktionslogik für LogEntryWindow.xaml
    /// </summary>
    public partial class LogEntryWindow : Window
    {
        public LogEntryWindow(LogEntryView view, ISkinManager manager)
        {
            InitializeComponent();
            Content = view;
            manager.Apply(this);
        }
    }
}
