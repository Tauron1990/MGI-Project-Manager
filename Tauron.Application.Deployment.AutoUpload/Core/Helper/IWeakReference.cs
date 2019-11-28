namespace Tauron.Application.Deployment.AutoUpload.Core.Helper
{
    public interface IInternalWeakReference
    {
        bool IsAlive { get; }
    }
}