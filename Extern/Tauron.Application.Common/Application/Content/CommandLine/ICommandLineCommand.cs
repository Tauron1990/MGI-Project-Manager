#region

using JetBrains.Annotations;
using Tauron.Application.Ioc;

#endregion

namespace Tauron.Application
{
    /// <summary>The CommandLineCommand interface.</summary>
    [PublicAPI]
    public interface ICommandLineCommand
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The execute.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <param name="container">
        ///     The container.
        /// </param>
        void Execute([NotNull] string[] args, [NotNull] IContainer container);

        #endregion

        #region Public Properties

        /// <summary>Gets the command name.</summary>
        /// <value>The command name.</value>
        [NotNull]
        string CommandName { get; }

        /// <summary>Gets the factory.</summary>
        /// <value>The factory.</value>
        [CanBeNull]
        IShellFactory Factory { get; }

        #endregion
    }
}