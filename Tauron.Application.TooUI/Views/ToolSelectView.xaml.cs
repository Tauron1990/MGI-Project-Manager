using System.Windows.Media;
using Tauron.Application.ToolUI.ViewModels;
using Tauron.Application.Wpf;

namespace Tauron.Application.ToolUI.Views
{
    /// <summary>
    /// Interaktionslogik für ToolSelectView.xaml
    /// </summary>
    [Control(typeof(ToolSelectViewModel))]
    public partial class ToolSelectView
    {
        public ToolSelectView(ToolSelectViewModel model)
            : base(model)
        {
            InitializeComponent();

            Background = Brushes.Transparent;
        }
    }
}
