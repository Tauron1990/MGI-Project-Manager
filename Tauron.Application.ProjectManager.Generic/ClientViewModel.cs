using System;
using System.Collections.Generic;
using System.ServiceModel;
using Tauron.Application.Ioc;
using Tauron.Application.Models;
using Tauron.Application.ProjectManager.Resources;
using Tauron.Application.ProjectManager.Services.DTO;
using Tauron.Application.ProjectManager.UI;

namespace Tauron.Application.ProjectManager.Generic
{
    public abstract class ClientViewModel : ViewModelBase, IDisposable
    {
        private ClientException _clientException;
        private Exception       _openException;
        private bool            _statusOk;

        protected Dictionary<Type, ClientObjectBase> ClientObjects = new Dictionary<Type, ClientObjectBase>();

        public bool StatusOk
        {
            get => _statusOk;
            set => SetProperty(ref _statusOk, value);
        }

        public Exception OpenException
        {
            get => _openException;
            set => SetProperty(ref _openException, value);
        }

        public ClientException ClientException
        {
            get => _clientException;
            private set => SetProperty(ref _clientException, value);
        }

        [Inject]
        public IClientFactory ClientFactory { get; set; }

        public void Dispose()
        {
            foreach (var clientObject in ClientObjects)
            {
                (clientObject.Value.CommunicationObject as IDisposable)?.Dispose();
            }
        }

        protected void ResetClients()
        {
            foreach (var clientObjectBase in ClientObjects)
            {
                if(clientObjectBase.Value.State == CommunicationState.Opened || clientObjectBase.Value.State == CommunicationState.Opening)
                    clientObjectBase.Value.CommunicationObject.Close();
            }

            ClientObjects.Clear();
        }

        protected TClient CreateClint<TClient>() where TClient : class
        {
            var key = typeof(TClient);

            if (ClientObjects.TryGetValue(key, out var clientObjectBase))
                if (clientObjectBase.State != CommunicationState.Faulted && clientObjectBase is ClientObject<TClient> tempClient)
                    return tempClient.Client;

            var client = ClientFactory.CreateClient<TClient>();
            if (client == null) return null;

            ClientObjects[key] = client;
            return client.Client;
        }

        protected void Close(params Type[] clients)
        {
            if (clients == null)
                foreach (var clientObject in ClientObjects)
                {
                    if (clientObject.Value.State != CommunicationState.Closed)
                        clientObject.Value.Close();
                }
            else
                foreach (var client in clients)
                {
                    if (!ClientObjects.TryGetValue(client, out var clientObjectBase)) continue;

                    if (clientObjectBase.State != CommunicationState.Closed)
                        clientObjectBase.Close();
                }
        }

        protected bool EnsureOpen(params Type[] clients)
        {
            try
            {
                var clientTypes = new List<Type>();
                if (clients == null) clientTypes.AddRange(ClientObjects.Keys);
                else clientTypes.AddRange(clients);

                foreach (var clientType in clientTypes)
                {
                    if (!ClientObjects.TryGetValue(clientType, out var objectBase) || objectBase.State == CommunicationState.Opened) continue;

                    BeginOpen();
                    objectBase.Open();
                    ConnectionEstablished(clientType, objectBase);
                }

                StatusOk = true;
                return true;
            }
            catch (Exception e)
            {
                if (CriticalExceptions.IsCriticalApplicationException(e)) throw;

                if (e.InnerException != null)
                    e = e.InnerException;

                OpenException = e;
                StatusOk      = false;

                OpenFailed();

                return false;
            }
        }

        protected TResult Secure<TResult>(Func<TResult> action, out bool ok)
        {
            try
            {
                var temp = action();
                ok = true;

                return temp;
            }
            catch (ClientException e)
            {
                ok              = false;
                ClientException = e;
                StatusOk = false;
                return default(TResult);
            }
        }

        protected bool Secure(Action action)
        {
            try
            {
                action();
                return true;
            }
            catch (ClientException e)
            {
                ClientException = e;
                StatusOk = false;
                return false;
            }
        }

        protected string ProcessDefaultErrors()
        {
            if (ClientException == null) return string.Empty;

            var inner = ClientException.InnerException;

            switch (inner)
            {
                case FaultException<GenericServiceFault> genericFaultException:
                    Dialogs.ShowMessageBox(MainWindow, genericFaultException.Detail.Reason, "Error", MsgBoxButton.Ok, MsgBoxImage.Error, null);
                    return genericFaultException.Detail.Reason;
                case FaultException<LogInFault> loginFaultException:
                    Dialogs.ShowMessageBox(MainWindow, loginFaultException.Detail.Reason, UIMessages.LogIn_Error_Caption, MsgBoxButton.Ok, MsgBoxImage.Error, null);
                    return loginFaultException.Detail.Reason;
                case CommunicationException communicationException:
                    Dialogs.FormatException(MainWindow, communicationException);
                    return $"{inner.GetType()} - {communicationException.Message}";
            }

            return inner?.Message ?? string.Empty;
        }

        protected virtual void OpenFailed()
        {

        }

        protected virtual void BeginOpen()
        {
        }

        protected virtual void ConnectionEstablished(Type type, ClientObjectBase clientObjectBase)
        {
        }
    }
}