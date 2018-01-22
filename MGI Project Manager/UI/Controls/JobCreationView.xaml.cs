using System.Windows.Controls;
using Tauron.Application.Views;

namespace Tauron.Application.MgiProjectManager.UI.Controls
{
    /// <summary>
    ///     Interaktionslogik für JobCreationView.xaml
    /// </summary>
    [ExportView(AppConststands.CreateJobControl)]
    public partial class JobCreationView : UserControl
    {
        public JobCreationView()
        {
            InitializeComponent();
        }
    }
}