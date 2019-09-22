using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ServiceManager
{
    /// <summary>
    /// Interaktionslogik für ValueRequesterWindow.xaml
    /// </summary>
    public partial class ValueRequesterWindow
    {
        public ValueRequesterWindow(string message)
        {
            InitializeComponent();
            
            Icon = BitmapFrame.Create(File.Open("AppIcon.ico", FileMode.Open));
            Message.Text = message;
        }

        public string Result { get; private set; }

        public bool Shutdown { get; private set; }

        private void Ok_OnClick(object sender, RoutedEventArgs e)
        {
            Result = ResultBox.Text;
            DialogResult = true;
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e) 
            => DialogResult = false;

        private void ShutdownClick(object sender, RoutedEventArgs e)
        {
            Shutdown = true;
            DialogResult = false;
        }
    }
}
