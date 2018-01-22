#region

using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The UIControllerFactory interface.</summary>
    public interface IUIControllerFactory
    {
        #region Public Methods and Operators

        /// <summary>The create controller.</summary>
        /// <returns>
        ///     The <see cref="IUIController" />.
        /// </returns>
        [NotNull]
        IUIController CreateController();

        /// <summary>The set synchronization context.</summary>
        void SetSynchronizationContext();

        #endregion
    }
}