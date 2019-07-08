using System.Threading;

namespace Tauron.CQRS.Server.Handler
{
    public interface IGenericHandler
    {
        void Invoke(object arg);
        void Invoke(object arg, CancellationToken token);
    }
}