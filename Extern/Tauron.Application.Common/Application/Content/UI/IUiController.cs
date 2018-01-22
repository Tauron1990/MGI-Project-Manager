#region

using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The shutdown mode.</summary>
    [PublicAPI]
    public enum ShutdownMode
    {
        /// <summary>The on last window close.</summary>
        OnLastWindowClose,

        /// <summary>The on main window close.</summary>
        OnMainWindowClose,

        /// <summary>The on explicit shutdown.</summary>
        OnExplicitShutdown
    }

    /// <summary>The UIController interface.</summary>
    [PublicAPI]
    public interface IUIController
    {
        #region Public Properties

        /// <summary>Gets or sets the main window.</summary>
        /// <value>The main window.</value>
        [CanBeNull]
        IWindow MainWindow { get; set; }

        /// <summary>Gets or sets the shutdown mode.</summary>
        /// <value>The shutdown mode.</value>
        ShutdownMode ShutdownMode { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The run.
        /// </summary>
        /// <param name="window">
        ///     The window.
        /// </param>
        void Run([CanBeNull] IWindow window);

        /// <summary>The shutdown.</summary>
        void Shutdown();

        #endregion
    }
}