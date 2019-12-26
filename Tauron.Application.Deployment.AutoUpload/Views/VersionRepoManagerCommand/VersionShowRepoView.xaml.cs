using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tauron.Application.Deployment.AutoUpload.ViewModels.VersionRepoManagerCommand;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.Views.VersionRepoManagerCommand
{
    /// <summary>
    /// Interaktionslogik für VersionShowRepoView.xaml
    /// </summary>
    [Control(typeof(VersionNewRepoViewModel))]
    public partial class VersionShowRepoView
    {
        public VersionShowRepoView(VersionNewRepoViewModel model)
            : base(model)
        {
            InitializeComponent();
            Background = Brushes.Transparent;
        }
    }
}
