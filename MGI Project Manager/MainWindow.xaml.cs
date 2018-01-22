using System.Windows;
using System.Windows.Data;
using Syncfusion.Windows.Controls.Input;
using Tauron.Application.MgiProjectManager.UI.Model;
using Tauron.Application.Models;
using Tauron.Application.Views;

namespace Tauron.Application.MgiProjectManager
{
    /// <summary>
    ///     Interaktionslogik für MainWindow.xaml
    /// </summary>
    [ExportWindow(AppConststands.MainWindowName)]
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MaskEditorVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (SfMaskedEdit) sender;

            if (control.Visibility == Visibility.Visible) control.Focus();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            ((WorkItemModel) ModelBase.ResolveModel(AppConststands.WorkItemModel)).CollectionView = ((CollectionViewSource) Resources["JobSource"]).View;
        }
    }
}