using JetBrains.Annotations;

namespace Tauron
{
    [PublicAPI]
    public interface IWeakReference
    {
        bool IsAlive { get; }
    }
}