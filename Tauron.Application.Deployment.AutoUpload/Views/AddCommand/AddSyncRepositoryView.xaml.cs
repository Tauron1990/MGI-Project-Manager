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
using Tauron.Application.Deployment.AutoUpload.ViewModels.AddCommand;
using Tauron.Application.Wpf;

namespace Tauron.Application.Deployment.AutoUpload.Views.AddCommand
{
    /// <summary>
    /// Interaktionslogik für AddSyncRepositoryView.xaml
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
