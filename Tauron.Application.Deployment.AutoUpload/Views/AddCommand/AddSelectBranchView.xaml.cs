using System.Windows.Media;
using Tauron.Application.Deployment.AutoUpload.ViewModels.AddCommand;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.Views.AddCommand
{
    /// <summary>
    ///     Interaktionslogik für AddSelectBranchView.xaml
    /// </summary>
    [Control(typeof(AddSelectBranchViewModel))]
    public partial class AddSelectBranchView
    {
        public AddSelectBranchView(AddSelectBranchViewModel model)
            : base(model)
        {
            InitializeComponent();
            Background = Brushes.Transparent;
        }
    }
}