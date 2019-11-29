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
using Tauron.Application.Deployment.AutoUpload.ViewModels.RemoveCommand;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.Views.RemoveCommand
{
    /// <summary>
    /// Interaktionslogik für RemoveSelectProjectView.xaml
    /// </summary>
    [Control(typeof(RemoveSelectProjectViewModel))]
    public partial class RemoveSelectProjectView
    {
        public RemoveSelectProjectView(RemoveSelectProjectViewModel model)
        : base(model)
        {
            InitializeComponent();
            Background = Brushes.Transparent;
        }
    }
}
