using System;
using System.ComponentModel;
using Tauron.Application.Ioc;
using Tauron.Application.Models;
using Tauron.Application.ProjectManager.UI;

namespace Tauron.Application.ProjectManager.Generic
{
    public abstract class ClientViewModel : ViewModelBase, IDisposable
    {
        private ServiceManager _serviceManager;

        public bool StatusOk
        {
            get => _serviceManager.StatusOk;
            set => _serviceManager.StatusOk = value;
        }

        public Exception OpenException => _serviceManager.OpenException;

        public ClientException ClientException => _serviceManager.ClientException;

        [Inject]
        public IClientFactory ClientFactory { get; set; }

        public void Dispose()
        {
            _serviceManager.PropertyChanged       -= ServiceManagerOnPropertyChanged;
            _serviceManager.OpenFailed            -= OpenFailed;
            _serviceManager.BeginOpen             -= BeginOpen;
            _serviceManager.ConnectionEstablished -= ConnectionEstablished;
            _serviceManager.Dispose();
        }

        protected void ResetClients() => _serviceManager.ResetClients();

        protected TClient CreateClint<TClient>() where TClient : class => _serviceManager.CreateClint<TClient>();

        protected void Close(params Type[] clients) => _serviceManager.Close(clients);

        protected bool EnsureOpen(params Type[] clients) => _serviceManager.EnsureOpen(clients);

        protected TResult Secure<TResult>(Func<TResult> action, out bool ok) => _serviceManager.Secure(action, out ok);

        protected bool Secure(Action action) => _serviceManager.Secure(action);

        protected string ProcessDefaultErrors() => _serviceManager.ProcessDefaultErrors();

        protected virtual void OpenFailed()
        {

        }

        protected virtual void BeginOpen()
        {
        }

        protected virtual void ConnectionEstablished(ServiceManager serviceManager, Type type, ClientObjectBase clientObjectBase)
        {
        }

        public override void BuildCompled()
        {
            base.BuildCompled();

            _serviceManager                       =  new ServiceManager(Dialogs, MainWindow);
            _serviceManager.PropertyChanged       += ServiceManagerOnPropertyChanged;
            _serviceManager.OpenFailed            += OpenFailed;
            _serviceManager.BeginOpen             += BeginOpen;
            _serviceManager.ConnectionEstablished += ConnectionEstablished;
        }

        private void ServiceManagerOnPropertyChanged(object sender, PropertyChangedEventArgs e) => OnPropertyChangedExplicit(e.PropertyName);
    }
}