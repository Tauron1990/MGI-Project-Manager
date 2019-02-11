using System;
using System.ServiceModel;

namespace Tauron.Application.ProjectManager.UI
{
    public class ClientObject<TClient> : ClientObjectBase where TClient : class
    {
        public ClientObject(ITypedClientHelperBase<TClient> client)
            : base(client)
        {
            Client = client.Client;
        }



        public TClient Client { get; }
    }

    public abstract class ClientObjectBase
    {
        private readonly IClientHelperBase _clientHelperBase;

        protected ClientObjectBase(IClientHelperBase client)
        {
            _clientHelperBase = client ?? throw new ArgumentNullException(nameof(client));
        }

        public ICommunicationObject CommunicationObject => _clientHelperBase;

        public CommunicationState State => CommunicationObject.State;

        public string Name
        {
            get => _clientHelperBase.Name;
            set => _clientHelperBase.Name = value;
        }

        public string Password
        {
            get => _clientHelperBase.Password;
            set => _clientHelperBase.Password = value;
        }

        public void Open() => CommunicationObject.Open();

        public void Close() => CommunicationObject.Close();
    }
}