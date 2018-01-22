#region

using System.ComponentModel;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Aop.Model
{
    /// <summary>The NotifyPropertyChangedMethod interface.</summary>
    [PublicAPI]
    public interface INotifyPropertyChangedMethod : INotifyPropertyChanged
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The on property changed.
        /// </summary>
        /// <param name="eventArgs">
        ///     The event args.
        /// </param>
        void OnPropertyChanged([NotNull] string eventArgs);

        #endregion
    }
}