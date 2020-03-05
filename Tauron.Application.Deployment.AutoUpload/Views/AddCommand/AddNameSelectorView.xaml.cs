using System.Windows.Media;
using Tauron.Application.Deployment.AutoUpload.ViewModels.AddCommand;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.Views.AddCommand
{
    /// <summary>
    ///     Interaktionslogik für AddNameSelectorView.xaml
    /// </summary>
    [Control(typeof(AddNameSelectorViewModel))]
    public partial class AddNameSelectorView
    {
        public AddNameSelectorView(AddNameSelectorViewModel model)
            : base(model)
        {
            InitializeComponent();
            Background = Brushes.Transparent;
        }
    }
}