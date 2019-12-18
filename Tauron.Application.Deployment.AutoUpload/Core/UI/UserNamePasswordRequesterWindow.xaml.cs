using System;
using System.Security;
using System.Windows;
using Syncfusion.SfSkinManager;

namespace Tauron.Application.Deployment.AutoUpload.Core.UI
{
    /// <summary>
    /// Interaktionslogik für UserNamePasswordRequesterWindow.xaml
    /// </summary>
    public partial class UserNamePasswordRequesterWindow : Window
    {
        public UserNamePasswordRequesterWindow()
        {
            InitializeComponent();

            SfSkinManager.SetVisualStyle(this, VisualStyles.Blend);

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
    }
}
