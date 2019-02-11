using System;
using Tauron.Application.Views;

namespace Tauron.Application.ProjectManager.Generic.Windows
{
    /// <summary>
    ///     Interaktionslogik für LogInWindow.xaml
    /// </summary>
    [ExportWindow(Consts.LoginWindowName)]
    public partial class LogInWindow
    {
        public LogInWindow()
        {
            InitializeComponent();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            var showUserName = ((LogInWindowViewModel) DataContext).ShowUserName;

            if (showUserName)
                UserName.Focus();
            else
                Password.Focus();

            base.OnContentRendered(e);
        }
    }
}