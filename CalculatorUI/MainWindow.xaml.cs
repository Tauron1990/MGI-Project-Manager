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

namespace CalculatorUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowModel _model;

        public MainWindow() => InitializeComponent();

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _model = new MainWindowModel();

            await Dispatcher.InvokeAsync(() => DataContext = _model);

            await _model.Load();
        }

        private async void Eval_OnClick(object sender, RoutedEventArgs e) 
            => await _model.Eval();
    }
}
