using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NLog;
using Tauron.Application.Ioc;
using Tauron.Application.ProjectManager.Generic.Clients;
using Tauron.Application.ProjectManager.Generic.Windows;
using Tauron.Application.ProjectManager.Services;
using Tauron.Application.ProjectManager.UI;
using Tauron.Application.Views;

namespace Tauron.Application.ProjectManager.Generic
{
    [PublicAPI]
    [Export(typeof(IClientFactory))]
    public class ClientFactory : IClientFactory
    {
        private static WeakCollection<ClientObjectBase> _clientObjectBases = new WeakCollection<ClientObjectBase>();

        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private static Dictionary<Type, (bool, Type, string, Func<object>)> _clients = Initialize();

        public string Password { get; private set; }

        public string Name { get; private set; }

        public ClientObject<TClient> CreateClient<TClient>()
            where TClient : class
        {
            if (!_clients.TryGetValue(typeof(TClient), out var entry)) return null;

            var settings = IpSettings.ReadIpSettings();

            try
            {
                var parms = new List<object>();
                if (entry.Item1)
                    parms.Add(entry.Item4());
                parms.Add(LocationHelper.CreateBinding());
                parms.Add(new EndpointAddress(LocationHelper.BuildUrl(settings.NetworkTarget, entry.Item3)));

                var client = Activator.CreateInstance(entry.Item2, parms.ToArray());

                var credinals = (ClientHelperBase<TClient>) client;
                credinals.Name = Name;
                credinals.Password = Password;

                var temp = new ClientObject<TClient>(credinals);
                _clientObjectBases.Add(temp);
                return temp;
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Error Client Creation: {typeof(TClient)}");

                if (CriticalExceptions.IsCriticalApplicationException(e)) throw;

                return null;
            }
        }

        public Task<bool> ShowLoginWindow(IWindow mainWindow, bool asAdmin)
        {
            Name = Properties.Settings.Default.UserName;

            var settings = new LogInWindowSettings(Password, asAdmin ? "admin" : Name);

            var window = UiSynchronize.Synchronize.Invoke(() => ViewManager.Manager.CreateWindow(Consts.LoginWindowName, settings));

            return window.ShowDialogAsync(mainWindow).ContinueWith(t =>
                                                                   {
                                                                       if (window.DialogResult != true) return false;

                                                                       Name     = settings.UserName;
                                                                       Password = settings.Password;

                                                                       SetPassword();

                                                                       Properties.Settings.Default.UserName = Name;
                                                                       Properties.Settings.Default.Save();
                                                                       return true;
                                                                   });
        }

        public void ChangePassword(string password)
        {
            Password = password;
            SetPassword();
        }

        private void SetPassword()
        {
            foreach (var clientObjectBase in _clientObjectBases)
            {
                clientObjectBase.Close();

                clientObjectBase.Password = Password;
                clientObjectBase.Name = Name;
            }
        }

        private static Dictionary<Type, (bool, Type, string, Func<object>)> Initialize()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            ServicePointManager.CheckCertificateRevocationList      =  false;

            return new Dictionary<Type, (bool, Type, string, Func<object>)>
                   {
                       {typeof(IAdminService), (false, typeof(AdminClient), ServiceNames.AdminService, null)},
                       {typeof(IUserService), (true, typeof(UserClient), ServiceNames.UserService, () => new UserCallBack())}
                   };
        }
    }
}