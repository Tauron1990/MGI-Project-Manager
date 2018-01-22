#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITauronEnviroment.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The TauronEnviroment interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The TauronEnviroment interface.</summary>
    [PublicAPI]
    public interface ITauronEnviroment
    {
        #region Public Properties

        /// <summary>Gets or sets the default profile path.</summary>
        /// <value>The default profile path.</value>
        [NotNull]
        string DefaultProfilePath { get; set; }

        /// <summary>Gets the local application data.</summary>
        /// <value>The local application data.</value>
        [NotNull]
        string LocalApplicationData { get; }

        /// <summary>Gets the local application temp folder.</summary>
        /// <value>The local application temp folder.</value>
        [NotNull]
        string LocalApplicationTempFolder { get; }

        /// <summary>Gets the local download folder.</summary>
        /// <value>The local download folder.</value>
        [NotNull]
        string LocalDownloadFolder { get; }

        #endregion

        #region Public Methods and Operators

        [NotNull]
        IEnumerable<string> GetProfiles([NotNull] string application);

        /// <summary>
        ///     The search for folder.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull]
        string SearchForFolder(Guid id);

        #endregion
    }
}