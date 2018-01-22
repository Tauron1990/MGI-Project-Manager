#region

using Tauron.Application.Ioc;

#endregion

namespace Tauron.Application.Implement
{
    /// <summary>The clipboard manager.</summary>
    [Export(typeof(IClipboardManager))]
    public class ClipboardManager : IClipboardManager
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
        public ClipboardViewer CreateViewer(IWindow target, bool registerForClose, bool performInitialization)
        {
            return new ClipboardViewer(target, registerForClose, performInitialization);
        }

        #endregion
    }
}