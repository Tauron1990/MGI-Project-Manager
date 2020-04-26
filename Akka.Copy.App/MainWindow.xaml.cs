using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Akka.Copy.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private MainWindowViewModel Model => (MainWindowViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SearchBase(object sender, RoutedEventArgs e)
            => Model.SearchBase();

        private void UIElement_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] dics = (string[])e.Data.GetData(DataFormats.FileDrop);

                Model.AddDics(dics);
            }
        }

        private void UIElement_OnPreviewDragOver(object sender, DragEventArgs e)
            => e.Handled = true;

        private void SearchTarget(object sender, RoutedEventArgs e)
            => Model.SearchTarget();

        private void StartProgress(object sender, RoutedEventArgs e)
            => Model.StartProgress();

        private void LoadLast(object sender, RoutedEventArgs e)
            => Model.LoadLast();

        private void Stop(object sender, RoutedEventArgs e)
            => Model.Stop();
    }
}
