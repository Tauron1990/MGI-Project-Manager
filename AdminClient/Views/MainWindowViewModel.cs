using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.Threading.Tasks;
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
        private string        _currentPassword;
        private string        _currentUser;
        private string        _errorText;
        private IpSettings    _ipSettings;
        private bool          _isBusy;
        private bool          _needPasswordChange;
        private string        _networkTarget;
        private string        _newPassword;
        private IUserService  _userService;

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

        public string CurrentPassword
        {
            get => _currentPassword;
            set => SetProperty(ref _currentPassword, value);
        }

        public string NewPassword
        {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }

        public bool NeedPasswordChange
        {
            get => _needPasswordChange;
            set => SetProperty(ref _needPasswordChange, value);
        }

        public override void OnShow(IWindow window)
        {
            window.Closed += (sender, args) =>
                             {
                                 try
                                 {
                                     _adminService.AdminLogout();
                                 }
                                 catch (Exception e)
                                 {
                                     if (CriticalExceptions.IsCriticalApplicationException(e)) throw;
                                 }
                             };
        }

        public override void AfterShow(IWindow window) => InitalLoad();

        public override void BuildCompled()
        {
            IsBusy = true;

            _ipSettings   = IpSettings.ReadIpSettings();
            NetworkTarget = _ipSettings.NetworkTarget;

            base.BuildCompled();
        }

        [CommandTarget]
        public void Connect() => InitalLoad();

        [CommandTarget]
        public void ChangeAdminPassword()
        {
            if (!EnsureOpen(typeof(IUserService)))
            {
                ProcessOpenException();
                return;
            }

            var result = Secure(() => _userService.ChangePassword("admin", NewPassword, CurrentPassword), out var isok);

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

            ErrorText          = result.Reason;
            NeedPasswordChange = false;
        }

        [CommandTarget]
        public bool CanChangeAdminPassword() => !string.IsNullOrWhiteSpace(CurrentPassword) && !string.IsNullOrWhiteSpace(NewPassword);

        [CommandTarget]
        public void SaveIpSettings()
        {
            _ipSettings.NetworkTarget = NetworkTarget;
            IpSettings.WriteIpSettings(_ipSettings);
        }

        [CommandTarget]
        public void ReloadIpSettings() => NetworkTarget = _ipSettings.NetworkTarget;

        [CommandTarget]
        public void ExportIpSettings()
        {
            string localIp = null;
            try
            {
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    var endPoint = socket.LocalEndPoint as IPEndPoint;
                    localIp = endPoint?.Address.ToString();
                }
            }
            catch (Exception e)
            {
                if (CriticalExceptions.IsCriticalException(e)) throw;
            }

            var settings = IpSettings.ReadIpSettings();
            if (localIp != null)
                settings.NetworkTarget = localIp;

            var formatter = new BinaryFormatter();

            var file = Dialogs.ShowSaveFileDialog(MainWindow, true, true, true, ".nt", true, ".nt|Network", false, true, "Export", Directory.GetCurrentDirectory(), out var ok);
            if (ok != true) return;

            settings.Serialize(formatter, file);
            ErrorText = AdminClientLabels.Text_IpExport_Ok;
        }

        [CommandTarget]
        public void CreateUser()
        {
            if (OpenAdmin())
                return;

            var name = Dialogs.GetText(MainWindow, AdminClientLabels.Label_NewUser_Instruction_Text, null, "User Name", false, string.Empty);

            if (string.IsNullOrWhiteSpace(name)) return;

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
        public bool CanDeleteUser()
        {
            return !string.IsNullOrWhiteSpace(CurrentUser);
        }

        [CommandTarget]
        public void DeleteUser()
        {
            if (OpenAdmin()) return;

            var name = CurrentUser;
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
        public void LogIn() => InteralLogIn();

        private Task<bool> InteralLogIn() => ClientFactory.ShowLoginWindow(MainWindow, true);

        private bool GetBasicData()
        {
            IsBusy = true;
            try
            {
                while (true)
                {
                    if (!EnsureOpen(typeof(IUserService)))
                    {
                        ProcessOpenException();
                        return false;
                    }

                    IsBusy = true;
                    var users = Secure(() => _userService.GetUsers(), out var isOk);

                    if (!isOk)
                    {
                        ErrorText = ProcessDefaultErrors();
                        if (!(ClientException?.InnerException is FaultException<LogInFault>)) return false;

                        LogIn();
                        continue;
                    }

                    Users.AddRange(users.Where(str => str != "admin"));
                    ErrorText = "OK";
                    break;
                }
            }
            finally
            {
                IsBusy = false;
            }

            return true;
        }

        private bool OpenAdmin()
        {
            if (EnsureOpen(typeof(IAdminService))) return false;

            ProcessOpenException();

            return true;
        }

        private void ProcessOpenException()
        {
            if (OpenException == null) return;

            Dialogs.FormatException(MainWindow, OpenException);
            var ex = OpenException.InnerException;

            ErrorText = (ex ?? OpenException).Message;
        }

        private void InitalLoad()
        {
            ResetClients();
            _adminService = CreateClint<IAdminService>();
            _userService  = CreateClint<IUserService>();

            InteralLogIn().ContinueWith(t =>
                                        {
                                            if (!t.Result)
                                            {
                                                LogIn();
                                                StatusOk  = false;
                                                ErrorText = AdminClientLabels.Text_Login_Faild;
                                                IsBusy = false;
                                                return;
                                            }

                                            if (!GetBasicData())
                                                return;

                                            if (OpenAdmin()) return;
                                            NeedPasswordChange = _adminService.IsAdminPasswordNull();

                                            IsBusy = false;
                                        });
        }

        protected override void OpenFailed()
        {
            IsBusy = false;
            NeedPasswordChange = false;
        }

        protected override void BeginOpen() => IsBusy = true;

        protected override void ConnectionEstablished(Type type, ClientObjectBase clientObjectBase)
        {
            if (type == typeof(IAdminService))
                _adminService.AdminLogin(ClientFactory.Password);
            IsBusy = false;
        }
    }
}