using System.Windows.Controls;
using Tauron.Application.Views;

namespace Tauron.Application.MgiProjectManager.UI.Controls
{
    /// <summary>
    ///     Interaktionslogik für JobPreperationView.xaml
    /// </summary>
    [ExportView(AppConststands.JobPreperation)]
    public partial class JobPreperationView : UserControl
    {
        public JobPreperationView()
        {
            InitializeComponent();
        }
    }
}