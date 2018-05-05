using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using JetBrains.Annotations;
using NLog;
using Tauron.Application.ProjectManager.UI;

namespace Tauron.Application.ProjectManager.Generic.Clients
{
    public abstract class ClientHelperBase<TChannel> : ITypedClientHelperBase<TChannel> 
        where TChannel : class
    {
        private class CommonClient : ClientBase<TChannel>
        {
            public TChannel Proxy => Channel;

            public CommonClient(Binding binding, EndpointAddress adress)
                : base(binding, adress)
            {
                
            }

            public CommonClient(InstanceContext context, Binding binding, EndpointAddress adress)
                : base(context, binding, adress)
            {
                
            }
        }

        private readonly Binding _binding;
        private readonly EndpointAddress _adress;
        private readonly object _context;
        private CommonClient _channel;

        protected ClientHelperBase(Binding binding, EndpointAddress adress)

        {
            _binding = binding;
            _adress = adress;
            Logger = LogManager.GetCurrentClassLogger();
        }

        protected ClientHelperBase(object context, Binding binding, EndpointAddress adress)
            : this(binding, adress)
        {
            _context = context;
        }

        protected TChannel Channel => _channel?.Proxy;

        [PublicAPI]
        protected Logger Logger { get; }

        protected TReturn Secure<TReturn>(Func<TReturn> action)
        {
            try
            {
                return action();
            }
            catch (Exception e)
            {
                if (CriticalExceptions.IsCriticalException(e))
                    throw;

                Logger.Log(LogLevel.Error, e);

                Abort();
                throw new ClientException($"{e.GetType()} - {e.Message}", e);
            }
        }

        protected void Secure(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                if (CriticalExceptions.IsCriticalException(e))
                    throw;

                Logger.Log(LogLevel.Error, e);

                Abort();
                throw new ClientException($"{e.GetType()} - {e.Message}", e);
            }
        }

        public void Dispose()
        {
            Close();
        }

        public void Abort()
        {
            _channel?.Abort();
            _channel = null;
        }

        public void Close()
        {
            try
            {
                _channel?.Close();
            }
            catch
            {
                _channel?.Abort();
            }

            _channel = null;
        }

        void ICommunicationObject.Close(TimeSpan timeout) => ((ICommunicationObject) _channel)?.Close(timeout);

        IAsyncResult ICommunicationObject.BeginClose(AsyncCallback callback, object state) => ((ICommunicationObject) _channel)?.BeginClose(callback, state);

        IAsyncResult ICommunicationObject.BeginClose(TimeSpan timeout, AsyncCallback callback, object state) => ((ICommunicationObject) _channel)?.BeginClose(timeout, callback, state);

        void ICommunicationObject.EndClose(IAsyncResult result) => ((ICommunicationObject) _channel)?.EndClose(result);

        private CommonClient CreateClientBase()
        {
            var client = _context != null ? new CommonClient(new InstanceContext(_context), _binding, _adress) : new CommonClient(_binding, _adress);

            var credinals = client.ClientCredentials;

            if (credinals != null)
            {
                credinals.UserName.UserName = Name;
                credinals.UserName.Password = Password;

                credinals.ClientCertificate.Certificate                               = new X509Certificate2(Properties.Resources.ee, "tauron");
                credinals.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
            }

            return client;
        }

        public string Password { get; set; }

        public string Name { get; set; }

        public void Open()
        {
            if(_channel == null)
                _channel = CreateClientBase();
            _channel.Open();
        }

        public void Open(TimeSpan timeout)
        {
            if (_channel == null)
                _channel = CreateClientBase();
            ((ICommunicationObject) _channel).Open(timeout);
        }

        IAsyncResult ICommunicationObject.BeginOpen(AsyncCallback callback, object state) => ((ICommunicationObject) _channel)?.BeginOpen(callback, state);

        IAsyncResult ICommunicationObject.BeginOpen(TimeSpan timeout, AsyncCallback callback, object state) => ((ICommunicationObject) _channel)?.BeginOpen(timeout, callback, state);

        void ICommunicationObject.EndOpen(IAsyncResult result) => ((ICommunicationObject) _channel)?.EndOpen(result);

        public CommunicationState State => _channel?.State ?? CommunicationState.Closed;

        event EventHandler ICommunicationObject.Closed
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        event EventHandler ICommunicationObject.Closing
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        event EventHandler ICommunicationObject.Faulted
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        event EventHandler ICommunicationObject.Opened
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        event EventHandler ICommunicationObject.Opening
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        TChannel ITypedClientHelperBase<TChannel>.Client => this as TChannel;
    }
}