namespace Tauron.Application.ProjectManager.Generic.Clients
{
    public abstract class CallbackBase<TClient>
    {
        protected TClient Client { get; private set; }

        public void SetClient(TClient client) => Client = client;
    }
}