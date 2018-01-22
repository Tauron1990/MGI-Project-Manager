using JetBrains.Annotations;

namespace Tauron.Application
{
    /// <summary>The FileCommand interface.</summary>
    public interface IFileCommand
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The process file.
        /// </summary>
        /// <param name="file">
        ///     The file.
        /// </param>
        void ProcessFile([NotNull] string file);

        /// <summary>The provide factory.</summary>
        /// <returns>
        ///     The <see cref="IShellFactory" />.
        /// </returns>
        [NotNull]
        IShellFactory ProvideFactory();

        #endregion
    }
}