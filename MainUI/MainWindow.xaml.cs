using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Input;
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
            //MainGrid.RowDefinitions[3].SetBinding(RowDefinition.HeightProperty, new Binding
            //{
            //    Path = new PropertyPath("Height"),
            //    Source = MainGrid.RowDefinitions[0]
            //});
        }

        private void EventSetter_OnMouseEnterHandler(object sender, MouseEventArgs e) => ((ContentControl)sender).Background = Brushes.LightGray;

        private void EventSetter_OnMouseLeaveHandler(object sender, MouseEventArgs e) => ((ContentControl)sender).Background = Brushes.White;
    }
}