using System;
using System.ServiceModel;
using Tauron.Application.Models;
using Tauron.Application.ProjectManager.Generic;
using Tauron.Application.ProjectManager.Resources;
using Tauron.Application.ProjectManager.Services;
using Tauron.Application.ProjectManager.Services.DTO;
using Tauron.Application.ProjectManager.UI;

namespace Tauron.Application.ProjectManager.AdminClient.Views
{
    [ExportViewModel(AppConststands.MainWindowName)]
    public sealed class MainWindowViewModel : ClientViewModel
    {
        private IAdminService _adminService;
        private IUserService _userService;
        private string _errorText;
        private bool _isBusy;
        private string _networkTarget;
        private IpSettings _ipSettings;
        private string _currentUser;

        public UISyncObservableCollection<string> Users { get; } = new UISyncObservableCollection<string>();

        public string CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public string ErrorText
        {
            get => _errorText;
            set => SetProperty(ref _errorText, value);
        }

        public string NetworkTarget
        {
            get => _networkTarget;
            set => SetProperty(ref _networkTarget, value);
        }

        public override void OnShow(IWindow window)
        {
            window.Closed += (sender, args) =>
                             {
                                 try
                                 {
                                     _adminService.AdminLogout();
                                 }
                                 catch(Exception e)
                                 {
                                     if (CriticalExceptions.IsCriticalApplicationException(e)) throw;
                                 }
                             };
            LogIn();
            GetBasicData();
            IsBusy = false;
        }

        public override void BuildCompled()
        {
            IsBusy = true;

            _ipSettings = IpSettings.ReadIpSettings();
            NetworkTarget = _ipSettings.NetworkTarget;
            _adminService = CreateClint<IAdminService>();
            _userService = CreateClint<IUserService>();

            base.BuildCompled();
        }

        [CommandTarget]
        public void SaveIpSettings()
        {
            _ipSettings.NetworkTarget = NetworkTarget;
            IpSettings.WriteIpSettings(_ipSettings);
        }

        [CommandTarget]
        public void ReloadIpSettings() => NetworkTarget = _ipSettings.NetworkTarget;

        [CommandTarget]
        public void CreateUser()
        {
            if (OpenAdmin())
                return;

            string name = Dialogs.GetText(MainWindow, AdminClientLabels.Label_NewUser_Instruction_Text, null, "User Name", false, string.Empty);

            if(string.IsNullOrWhiteSpace(name)) return;

            var result = Secure(() => _adminService.CreateUser(name, name), out var isok);

            if (!isok)
            {
                ErrorText = ProcessDefaultErrors();
                return;
            }

            if (!result.SuccededSuccessful)
            {
                ErrorText = $"{AdminClientLabels.Label_Common_Error} {result.Reason}";
                return;
            }

            Users.Add(name);
            ErrorText = result.Reason;
        }

        [CommandTarget]
        public bool CanDeleteUser() => !string.IsNullOrWhiteSpace(CurrentUser);

        [CommandTarget]
        public void DeleteUser()
        {
            if(OpenAdmin()) return;

            string name = CurrentUser;
            CurrentUser = string.Empty;

            var result = Secure(() => _adminService.DeleteUser(name), out var isok);

            if (!isok)
            {
                ErrorText = ProcessDefaultErrors();
                return;
            }

            if (!result.SuccededSuccessful)
            {
                ErrorText = $"{AdminClientLabels.Label_Common_Error} {result.Reason}";
                return;
            }

            Users.Remove(name);
            ErrorText = result.Reason;
        }

        [CommandTarget]
        public void ReloadData()
        {
            IsBusy = true;
            GetBasicData();
            IsBusy = false;
        }

        [CommandTarget]
        public void LogIn()
        {
            bool loginNotOk;
            do
            {
                loginNotOk = !ClientFactory.ShowLoginWindow(MainWindow, true);
            } while (loginNotOk);
        }

        private void GetBasicData()
        {
            if (!EnsureOpen(typeof(IUserService)))
            {
                if(OpenException != null)
                    Dialogs.FormatException(MainWindow, OpenException);
                return;
            }
            string[] users = Secure(() => _userService.GetUsers(), out var isOk);

            if (!isOk)
            {
                ErrorText = ProcessDefaultErrors();
                if (!(ClientException?.InnerException is FaultException<LogInFault>)) return;

                LogIn();
                GetBasicData();
            }
            else
                Users.AddRange(users);
        }

        private bool OpenAdmin()
        {
            if (EnsureOpen(typeof(IAdminService))) return false;

            if(OpenException != null)
                Dialogs.FormatException(MainWindow, OpenException);

            return true;
        }

        protected override void ConnectionEstablished(Type type, ClientObjectBase clientObjectBase)
        {
            if(type == typeof(IAdminService))
                _adminService.AdminLogin(ClientFactory.Password);
        }
    }
}