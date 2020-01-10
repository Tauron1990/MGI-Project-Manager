using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tauron.Application.Deployment.AutoUpload.ViewModels.UploadCommand;
using Tauron.Application.Wpf;
using UserControl = System.Windows.Controls.UserControl;

namespace Tauron.Application.Deployment.AutoUpload.Views.UploadCommand
{
    /// <summary>
    /// Interaktionslogik für UploadSelectSoftwareRepoView.xaml
    /// </summary>
    [Control(typeof(UploadSelectSoftwareRepoViewModel))]
    public partial class UploadSelectSoftwareRepoView
    {
        public UploadSelectSoftwareRepoView(UploadSelectSoftwareRepoViewModel model)
            : base(model)
        {
            InitializeComponent();
            Background = Brushes.Transparent;
        }
    }
}
