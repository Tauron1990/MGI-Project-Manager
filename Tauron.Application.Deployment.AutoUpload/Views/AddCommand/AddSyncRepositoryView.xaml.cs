using System.Windows.Media;
using Tauron.Application.Deployment.AutoUpload.ViewModels.AddCommand;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.Views.AddCommand
{
    /// <summary>
    ///     Interaktionslogik für AddSyncRepositoryView.xaml
    /// </summary>
    [Control(typeof(AddSyncRepositoryViewModel))]
    public partial class AddSyncRepositoryView
    {
        public AddSyncRepositoryView(AddSyncRepositoryViewModel model)
            : base(model)
        {
            InitializeComponent();
            Background = Brushes.Transparent;
        }
    }
}