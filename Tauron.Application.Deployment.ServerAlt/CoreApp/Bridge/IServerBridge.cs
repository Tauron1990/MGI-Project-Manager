namespace Tauron.Application.Deployment.Server.CoreApp.Bridge
{
    public interface IServerBridge
    {
        IClientSetup ClientSetup { get; }
    }
}