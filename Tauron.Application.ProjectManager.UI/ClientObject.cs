using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Tauron.Application.ProjectManager.UI
{
    public class ClientObject<TClient> : ClientObjectBase where TClient : class
    {
        public ClientObject(TClient client)
            : base(client as ICommunicationObject, (client as ClientBase<TClient>)?.ClientCredentials, typeof(TClient))
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
        }



        public TClient Client { get; }
    }

    public abstract class ClientObjectBase
    {
        protected ClientObjectBase(ICommunicationObject client, ClientCredentials clientCredentials, Type clientType)
        {
            CommunicationObject = client ?? throw new ArgumentNullException(nameof(client));
            ClientCredentials   = clientCredentials;
            ClientType = clientType;
        }

        public Type ClientType { get; }
        public ClientCredentials    ClientCredentials   { get; }
        public ICommunicationObject CommunicationObject { get; }

        public CommunicationState State => CommunicationObject.State;

        

        public void Open() => CommunicationObject.Open();

        public void Close() => CommunicationObject.Close();
    }
}