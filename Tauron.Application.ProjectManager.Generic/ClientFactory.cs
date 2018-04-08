using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
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
    [PublicAPI, Export(typeof(IClientFactory))]
    public class ClientFactory : IClientFactory
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private static Dictionary<Type, (bool, Type, string, Func<object>)> _clients = Initialize();

        private static Dictionary<Type, (bool, Type, string, Func<object>)> Initialize()
        {
            return new Dictionary<Type, (bool, Type, string, Func<object>)>
                   {
                       {typeof(IAdminService), (false, typeof(AdminClient), ServiceNames.AdminService, null)},
                       {typeof(IUserService), (false, typeof(UserClient), ServiceNames.UserService, null)}
                   };
        }

        public string Password { get; private set; }

        public string Name { get; private set; }

        public ClientObject<TClient> CreateClient<TClient>()
            where TClient : class
        {
            if (!_clients.TryGetValue(typeof(TClient), out var entry)) return null;

            IpSettings settings = IpSettings.ReadIpSettings();

            try
            {
                var parms = new List<object>();
                if (entry.Item1)
                    parms.Add(new InstanceContext(entry.Item4()));
                parms.Add(LocationHelper.CreateBinding());
                parms.Add(new EndpointAddress(LocationHelper.BuildUrl(settings.NetworkTarget, entry.Item3)));

                var client = Activator.CreateInstance(entry.Item2, parms.ToArray());

                var credinals = (client as ClientBase<TClient>)?.ClientCredentials;
                if (credinals == null) return new ClientObject<TClient>(client as TClient);

                credinals.UserName.UserName = Name;
                credinals.UserName.Password = Password;

                return new ClientObject<TClient>(client as TClient);
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Error Client Creation: {typeof(TClient)}");

                if (CriticalExceptions.IsCriticalApplicationException(e)) throw;

                return null;
            }
        }

        public bool ShowLoginWindow(IWindow mainWindow, bool asAdmin)
        {
            var settings = new LogInWindowSettings(Password, asAdmin ? Name : "admin");

            var window = UiSynchronize.Synchronize.Invoke(() => ViewManager.Manager.CreateWindow(Consts.LoginWindowName, settings));

            window.ShowDialogAsync(mainWindow).Wait();

            if (window.DialogResult != true) return false;

            Name     = settings.UserName;
            Password = settings.Password;

            return true;

        }
    }
}