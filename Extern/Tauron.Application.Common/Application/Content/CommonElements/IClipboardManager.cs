#region

using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The ClipboardManager interface.</summary>
    public interface IClipboardManager
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The create viewer.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="registerForClose">
        ///     The register for close.
        /// </param>
        /// <param name="performInitialization">
        ///     The perform initialization.
        /// </param>
        /// <returns>
        ///     The <see cref="ClipboardViewer" />.
        /// </returns>
        [NotNull]
        ClipboardViewer CreateViewer([NotNull] IWindow target, bool registerForClose, bool performInitialization);

        #endregion
    }
}