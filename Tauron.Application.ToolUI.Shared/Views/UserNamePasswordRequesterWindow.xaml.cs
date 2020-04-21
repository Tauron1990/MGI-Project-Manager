using System.Security;
using System.Windows;
using Tauron.Application.ToolUI.Core;

namespace Tauron.Application.ToolUI.Views
{
    /// <summary>
    ///     Interaktionslogik für UserNamePasswordRequesterWindow.xaml
    /// </summary>
    public partial class UserNamePasswordRequesterWindow : Window
    {
        public UserNamePasswordRequesterWindow(ISkinManager skinManager)
        {
            InitializeComponent();

            skinManager.Apply(this);

            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Owner = System.Windows.Application.Current.MainWindow;
        }

        public SecureString? Password { get; private set; }

        public string? UserName { get; set; }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            PasswordBox.Text = string.Empty;
            DialogResult = false;
        }

        private void Ok_OnClick(object sender, RoutedEventArgs e)
        {
            Password = new SecureString();
            foreach (var c in PasswordBox.Text)
                Password.AppendChar(c);
            UserName = NameBox.Text;

            PasswordBox.Text = string.Empty;
            DialogResult = true;
        }

        private void UserNamePasswordRequesterWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            NameBox.Text = UserName;
        }
    }
}