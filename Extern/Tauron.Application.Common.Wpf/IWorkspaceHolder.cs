using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Tauron.Application
{
    /// <summary>The WorkspaceHolder interface.</summary>
    public interface IWorkspaceHolder
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The register.
        /// </summary>
        /// <param name="workspace">
        ///     The workspace.
        /// </param>
        void Register([NotNull] ITabWorkspace workspace);

        /// <summary>
        ///     The un register.
        /// </summary>
        /// <param name="workspace">
        ///     The workspace.
        /// </param>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Un")]
        void UnRegister([NotNull] ITabWorkspace workspace);

        #endregion
    }
}