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
using Tauron.Application.Deployment.AutoUpload.ViewModels.BuildCommand;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.Views.BuildCommand
{
    /// <summary>
    /// Interaktionslogik für BuildOpenLocationView.xaml
    /// </summary>
    [Control(typeof(BuildOpenLocationViewModel))]
    public partial class BuildOpenLocationView 
    {
        public BuildOpenLocationView(BuildOpenLocationViewModel model)
        : base(model)
        {
            InitializeComponent();
            Background = Brushes.Transparent;
        }
    }
}
