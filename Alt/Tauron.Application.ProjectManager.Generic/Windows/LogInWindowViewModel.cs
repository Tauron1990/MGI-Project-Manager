using Tauron.Application.Ioc;
using Tauron.Application.Models;

namespace Tauron.Application.ProjectManager.Generic.Windows
{
    [ExportViewModel(Consts.LoginWindowName)]
    public sealed class LogInWindowViewModel : ViewModelBase
    {
        private LogInWindowSettings _logInSettings;
        private IWindow             _window;

        [Inject]
        public LogInWindowViewModel(LogInWindowSettings settings)
        {
            _logInSettings = settings;

            UserName = _logInSettings.UserName;
            Password = _logInSettings.Password;

            ShowUserName = _logInSettings.UserName != "admin";
        }

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool ShowUserName { get; set; }

        public override void OnShow(IWindow window)
        {
            _window = window;
        }

        [CommandTarget]
        public void Cancel()
        {
            _window.DialogResult = false;
        }

        [CommandTarget]
        public void ComLogin()
        {
            _logInSettings.UserName = UserName;
            _logInSettings.Password = Password;

            _window.DialogResult = true;
        }

        [CommandTarget]
        public bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password);
        }
    }
}