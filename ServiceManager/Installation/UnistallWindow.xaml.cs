using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ServiceManager.Installation
{
    /// <summary>
    /// Interaktionslogik für UnistallWindow.xaml
    /// </summary>
    public partial class UnistallWindow : Window
    {
        public event Func<Task<bool>> StartEvent; 

        public UnistallWindow() => InitializeComponent();

        private async void UnistallWindow_OnLoaded(object sender, RoutedEventArgs e) => DialogResult = await StartEvent.Invoke();
    }
}
