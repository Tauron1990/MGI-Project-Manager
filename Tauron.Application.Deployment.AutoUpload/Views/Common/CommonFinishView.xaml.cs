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
using Tauron.Application.Deployment.AutoUpload.Core;
using Tauron.Application.Deployment.AutoUpload.ViewModels.Common;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.Views.Common
{
    /// <summary>
    /// Interaktionslogik für CommonFinishView.xaml
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
