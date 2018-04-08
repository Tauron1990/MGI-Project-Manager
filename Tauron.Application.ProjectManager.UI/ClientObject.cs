using System;
using System.ServiceModel;

namespace Tauron.Application.ProjectManager.UI
{
    public class ClientObject<TClient> : ClientObjectBase
    {
        public TClient Client { get; }

        public ClientObject(TClient client)
            :base(client as ICommunicationObject)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            Client = client;
        }
    }

    public abstract class ClientObjectBase
    {
        protected ClientObjectBase(ICommunicationObject client)
        {
            CommunicationObject = client ?? throw new ArgumentNullException(nameof(client));
        }

        public ICommunicationObject CommunicationObject { get; }

        public CommunicationState State => CommunicationObject.State;
        public void Open() => CommunicationObject.Open();
        public void Close() => CommunicationObject.Close();
    }
}