#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISingleInstanceApp.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The SingleInstanceApp interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The SingleInstanceApp interface.</summary>
    [PublicAPI]
    public interface ISingleInstanceApp
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The signal external command line args.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool SignalExternalCommandLineArgs([NotNull] IList<string> args);

        #endregion
    }
}