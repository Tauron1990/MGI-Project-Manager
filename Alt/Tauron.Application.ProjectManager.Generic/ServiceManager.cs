using System;
using System.Collections.Generic;
using System.ServiceModel;
using Tauron.Application.ProjectManager.Resources;
using Tauron.Application.ProjectManager.Services.DTO;
using Tauron.Application.ProjectManager.UI;

namespace Tauron.Application.ProjectManager.Generic
{
    public sealed class ServiceManager : ObservableObject, IDisposable
    {
        private static IClientFactory _clientFactory;

        public static IClientFactory ClientFactory => _clientFactory ??
                                                      (_clientFactory = CommonApplication.Current.Container
                                                          .Resolve<IClientFactory>());

        private readonly IDialogFactory _dialogFactory;
        private readonly IWindow _ownerWindow;
        private ClientException _clientException;
        private bool _statusOk;

        private readonly Dictionary<Type, ClientObjectBase> _clientObjects = new Dictionary<Type, ClientObjectBase>();

        public event Action<ServiceManager, Type, ClientObjectBase> ConnectionEstablished;
        public event Action BeginOpen;
        public event Action OpenFailed;

        public ServiceManager(IDialogFactory dialogFactory, IWindow ownerWindow)
        {
            _dialogFactory = dialogFactory;
            _ownerWindow = ownerWindow;
        }

        public bool StatusOk
        {
            get => _statusOk;
            set => SetProperty(ref _statusOk, value);
        }

        public Exception OpenException { get; private set; }

        public ClientException ClientException
        {
            get => _clientException;
            private set => SetProperty(ref _clientException, value);
        }


        public void Dispose()
        {
            foreach (var clientObject in _clientObjects)
                (clientObject.Value.CommunicationObject as IDisposable)?.Dispose();
        }

        public void ResetClients()
        {
            foreach (var clientObjectBase in _clientObjects)
                if (clientObjectBase.Value.State == CommunicationState.Opened ||
                    clientObjectBase.Value.State == CommunicationState.Opening)
                    clientObjectBase.Value.CommunicationObject.Close();

            _clientObjects.Clear();
        }

        public TClient CreateClint<TClient>() where TClient : class
        {
            var key = typeof(TClient);

            if (_clientObjects.TryGetValue(key, out var clientObjectBase))
                if (clientObjectBase is ClientObject<TClient> tempClient)
                    return tempClient.Client;

            var client = ClientFactory.CreateClient<TClient>();
            if (client == null) return null;

            _clientObjects[key] = client;
            return client.Client;
        }

        public void Close(params Type[] clients)
        {
            if (clients == null)
                foreach (var clientObject in _clientObjects)
                {
                    if (clientObject.Value.State != CommunicationState.Closed)
                        clientObject.Value.Close();
                }
            else
                foreach (var client in clients)
                {
                    if (!_clientObjects.TryGetValue(client, out var clientObjectBase)) continue;

                    if (clientObjectBase.State != CommunicationState.Closed)
                        clientObjectBase.Close();
                }
        }



        public bool EnsureOpen(params Type[] clients)
        {
            try
            {
                var clientTypes = new List<Type>();
                if (clients == null) clientTypes.AddRange(_clientObjects.Keys);
                else clientTypes.AddRange(clients);

                foreach (var clientType in clientTypes)
                {
                    if (!_clientObjects.TryGetValue(clientType, out var objectBase) ||
                        objectBase.State == CommunicationState.Opened) continue;

                    OnBeginOpen();
                    objectBase.Open();
                    OnConnectionEstablished(clientType, objectBase);
                }

                ClientException = null;
                StatusOk = true;
                return true;
            }
            catch (Exception e)
            {
                if (CriticalExceptions.IsCriticalApplicationException(e)) throw;

                if (e.InnerException != null)
                    e = e.InnerException;

                OpenException = e;
                StatusOk = false;

                OnOpenFailed();

                return false;
            }
        }

        public TResult Secure<TResult>(Func<TResult> action, out bool ok)
        {
            try
            {
                var temp = action();
                ok = true;

                return temp;
            }
            catch (ClientException e)
            {
                ok = false;
                ClientException = e;
                StatusOk = false;
                return default(TResult);
            }
        }

        public object Secure(Delegate action, out bool ok)
        {
            try
            {
                var temp = action.DynamicInvoke();
                ok = true;

                return temp;
            }
            catch (ClientException e)
            {
                ok = false;
                ClientException = e;
                StatusOk = false;
                return null;
            }
        }

        public bool Secure(Action action)
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

        public string ProcessDefaultErrors()
        {
            if (ClientException == null) return string.Empty;

            var inner = ClientException.InnerException;

            switch (inner)
            {
                case FaultException<GenericServiceFault> genericFaultException:
                    _dialogFactory.ShowMessageBox(_ownerWindow, genericFaultException.Detail.Reason, "Error",
                        MsgBoxButton.Ok, MsgBoxImage.Error, null);
                    return genericFaultException.Detail.Reason;
                case FaultException<LogInFault> loginFaultException:
                    _dialogFactory.ShowMessageBox(_ownerWindow, loginFaultException.Detail.Reason,
                        UIMessages.LogIn_Error_Caption, MsgBoxButton.Ok, MsgBoxImage.Error, null);
                    return loginFaultException.Detail.Reason;
                case CommunicationException communicationException:
                    _dialogFactory.FormatException(_ownerWindow, communicationException);
                    return $"{inner.GetType()} - {communicationException.Message}";
            }

            return inner?.Message ?? string.Empty;
        }

        private void OnOpenFailed() => OpenFailed?.Invoke();

        private void OnBeginOpen() => BeginOpen?.Invoke();

        private void OnConnectionEstablished(Type type, ClientObjectBase clientObjectBase) =>
            ConnectionEstablished?.Invoke(this, type, clientObjectBase);

        public override string ToString()
        {
            if (OpenException != null)
                return $"{OpenException.GetType().Name} - {OpenException.Message}";

            return base.ToString();
        }
    }
}