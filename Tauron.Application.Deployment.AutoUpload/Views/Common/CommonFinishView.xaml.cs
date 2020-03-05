using System.Windows.Media;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Common;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.Views.Common
{
    /// <summary>
    ///     Interaktionslogik für CommonFinishView.xaml
    /// </summary>
    [Control(typeof(CommonFinishViewModel))]
    public partial class CommonFinishView
    {
        public CommonFinishView(CommonFinishViewModel model)
            : base(model)
        {
            InitializeComponent();
            Background = Brushes.Transparent;
        }
    }
}