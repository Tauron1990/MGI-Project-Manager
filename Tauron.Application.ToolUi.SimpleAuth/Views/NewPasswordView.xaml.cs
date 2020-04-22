using System.Windows.Media;
using Tauron.Application.Wpf;

namespace Tauron.Application.ToolUi.SimpleAuth.Views
{
    /// <summary>
    /// Interaktionslogik für NewPasswordView.xaml
    /// </summary>
    [Control(typeof(NewPasswordViewModel))]
    public partial class NewPasswordView
    {
        public NewPasswordView(NewPasswordViewModel model)
            : base(model)
        {
            InitializeComponent();

            Background = Brushes.Transparent;
        }
    }
}
