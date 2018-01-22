#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommandLineService.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The CommandLineService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The CommandLineService interface.</summary>
    [PublicAPI]
    public interface ICommandLineService
    {
        #region Public Properties

        /// <summary>Gets the commands.</summary>
        /// <value>The commands.</value>
        [NotNull]
        IEnumerable<ICommandLineCommand> Commands { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add.
        /// </summary>
        /// <param name="command">
        ///     The command.
        /// </param>
        void Add([NotNull] ICommandLineCommand command);

        #endregion
    }
}