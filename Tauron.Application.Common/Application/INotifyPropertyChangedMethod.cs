using System.ComponentModel;
using JetBrains.Annotations;

namespace Tauron.Application
{
    /// <summary>The NotifyPropertyChangedMethod interface.</summary>
    [PublicAPI]
    public interface INotifyPropertyChangedMethod : INotifyPropertyChanged
    {
        void OnPropertyChanged([NotNull] string eventArgs);
    }
}