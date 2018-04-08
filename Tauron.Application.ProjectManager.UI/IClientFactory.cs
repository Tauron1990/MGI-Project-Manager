namespace Tauron.Application.ProjectManager.UI
{
    public interface IClientFactory
    {
        string Password { get; }

        string Name { get; }

        ClientObject<TClient> CreateClient<TClient>()  where TClient : class;

        bool ShowLoginWindow(IWindow mainWindow, bool asAdmin);
    }
}
