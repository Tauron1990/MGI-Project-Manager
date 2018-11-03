using JetBrains.Annotations;

namespace Tauron.Application
{
    public interface IResultProvider
    {
        [CanBeNull] object Result { get; }
    }
}