using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.Threading.Tasks;
using Tauron.Application.Models;
using Tauron.Application.ProjectManager.Generic;
using Tauron.Application.ProjectManager.Generic.Extensions;
using Tauron.Application.ProjectManager.Resources;
using Tauron.Application.ProjectManager.Services;
using Tauron.Application.ProjectManager.Services.DTO;
using Tauron.Application.ProjectManager.UI;

namespace Tauron.Application.ProjectManager.AdminClient.Views
{
    [ExportViewModel(AppConststands.MainWindowName)]
    public sealed class MainWindowViewModel : ClientViewModel
    {
        private ResultAwaiter<GenericServiceResult> _passwordChangeResult = new ResultAwaiter<GenericServiceResult>
                                                                            {
                                                                                TimeOut = TimeSpan.FromMilliseconds(5000), 
                                                                                DefaultValue = new GenericServiceResult(false, AdminClientLabels.Text_PassordChange_Timeout)
                                                                            };
        private IAdminService _adminService;
        private string        _currentPassword;
        private ServerUser        _currentUser;
        private string        _errorText;
        private IpSettings    _ipSettings;
        private bool          _isBusy;
        private bool          _needPasswordChange;
        private string        _networkTarget;
        private string        _newPassword;
        private IUserService  _userService;
        private string _newUserName;

        public UISyncObservableCollection<ServerUser> Users { get; } = new UISyncObservableCollection<ServerUser>();

        public ServerUser CurrentUser
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
            set => SetProperty(ref _networkTarget, value, () =>
                                                          {
                                                              try
                                                              {
                                                                  if (NetworkTarget == "localhost" || IPAddress.TryParse(NetworkTarget, out _))
                                                                  {
                                                                      ErrorText = "OK";
                                                                      return;
                                                                  }

                                                                  ErrorText = AdminClientLabels.Text_WrongIPSetting;
                                                              }
                                                              finally
                                                              {
                                                                  InvalidateRequerySuggested();
                                                              }
                                                          });
        }

        public string NewUserName
        {
            get => _newUserName;
            set => SetProperty(ref _newUserName, value);
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

        public override void AfterShow(IWindow window)
        {
            InitalLoad();
        }

        public override void BuildCompled()
        {
            IsBusy = true;

            _ipSettings   = IpSettings.ReadIpSettings();
            NetworkTarget = _ipSettings.NetworkTarget;

            base.BuildCompled();
        }

        [CommandTarget]
        public void Connect()
        {
            InitalLoad();
        }

        [CommandTarget]
        public void ChangeAdminPassword()
        {
            base.BuildCompled();

            IsBusy = true;

            Task.Run(() =>
                     {
                         if (!OpenAdmin())
                             _adminService.AdminLogout();
                         if (!EnsureOpen(typeof(IUserService)))
                         {
                             ProcessOpenException();
                             return;
                         }

                         bool isok = Secure(() => _userService.ChangePassword("admin", NewPassword, CurrentPassword));

                         if (!isok)
                         {
                             ErrorText = ProcessDefaultErrors();
                             return;
                         }

                         var result = _passwordChangeResult.Result;
                         _passwordChangeResult.Reset();


                         if (!result.SuccededSuccessful)
                         {
                             ErrorText = $"{AdminClientLabels.Label_Common_Error} {result.Reason}";
                             return;
                         }


                         ClientFactory.ChangePassword(NewPassword);
                         if (OpenAdmin())
                             return;

                         _adminService.AdminLogin(NewPassword);

                         ErrorText          = result.Reason;
                         NeedPasswordChange = false;
                         CurrentPassword    = string.Empty;
                         NewPassword        = string.Empty;

                     }).ContinueWith(t => IsBusy = false);
        }

        [CommandTarget]
        public bool CanChangeAdminPassword()
        {
            return !string.IsNullOrWhiteSpace(NewPassword);
        }

        [CommandTarget]
        public bool CanSaveIpSettings()
        {
            return NetworkTarget == "localhost" || IPAddress.TryParse(NetworkTarget, out _);
        }

        [CommandTarget]
        public void SaveIpSettings()
        {
            _ipSettings.NetworkTarget = NetworkTarget;
            IpSettings.WriteIpSettings(_ipSettings);
        }

        [CommandTarget]
        public void ReloadIpSettings()
        {
            NetworkTarget = _ipSettings.NetworkTarget;
        }

        [CommandTarget]
        public bool CanExportIpSettings()
        {
            return CanSaveIpSettings();
        }

        [CommandTarget]
        public void ExportIpSettings()
        {
            var    settings = _ipSettings;
            string localIp  = null;

            try
            {
                if (settings.NetworkTarget == "localhost")
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

            var name = string.IsNullOrWhiteSpace(NewUserName) ? 
                           Dialogs.GetText(MainWindow, AdminClientLabels.Label_NewUser_Instruction_Text, null, "User Name", false, string.Empty) :
                           NewUserName;

            NewUserName = string.Empty;

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

            Users.Add(new ServerUser(name, UserRights.Manager, UserRightsChanged));
            ErrorText = result.Reason;
        }

        private void UserRightsChanged(UserRights newR,  ServerUser user)
        {
            if (!OpenAdmin())
            {
                if (Secure(() => _adminService.SetUserRights(user.User, newR))) return;

                ErrorText = ProcessDefaultErrors();
                user.Revert();
            }
            else
            {
                user.Revert();
            }
        }

        [CommandTarget]
        public bool CanDeleteUser() => !string.IsNullOrWhiteSpace(CurrentUser?.User);

        [CommandTarget]
        public void DeleteUser()
        {
            if (OpenAdmin()) return;

            var name = CurrentUser;
            CurrentUser = null;

            var result = Secure(() => _adminService.DeleteUser(name.User), out var isok);

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
                    List<ServerUser> usersList = new List<ServerUser>();

                    #if DEBUG
                    Debug.Print($"User Count: {users.Length}");
                    #endif

                    if (!isOk)
                    {
                        ErrorText = ProcessDefaultErrors();
                        if (!(ClientException?.InnerException is FaultException<LogInFault>)) return false;

                        LogIn();
                        continue;
                    }

                    foreach (var user in users)
                    {
                        if (user == "admin") continue;

                        var right = Secure(() => _userService.GetUserRights(user), out isOk);
                        if (isOk)
                            usersList.Add(new ServerUser(user, right, UserRightsChanged));
                        else
                            break;
                    }

                    if (!isOk)
                    {
                        ErrorText = ProcessDefaultErrors();
                        return false;
                    }

                    Users.Clear();
                    Users.AddRange(usersList);
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
            ((IUserServiceExtension) _userService).PasswordChanged += result => _passwordChangeResult.SetResult(result);

            InteralLogIn().ContinueWith(t =>
                                        {
                                            if (!t.Result)
                                            {
                                                LogIn();
                                                StatusOk  = false;
                                                ErrorText = AdminClientLabels.Text_Login_Faild;
                                                IsBusy    = false;
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
            IsBusy             = false;
            NeedPasswordChange = false;
        }

        protected override void BeginOpen() => IsBusy = true;

        protected override void ConnectionEstablished(ServiceManager manager, Type type, ClientObjectBase clientObjectBase)
        {
            if (type == typeof(IAdminService))
                _adminService.AdminLogin(ClientFactory.Password);
            IsBusy = false;
        }
    }
}