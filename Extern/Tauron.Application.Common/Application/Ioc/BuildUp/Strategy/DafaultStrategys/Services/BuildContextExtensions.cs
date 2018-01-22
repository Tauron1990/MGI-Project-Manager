// The file BuildContextExtensions.cs is part of Tauron.Application.Common.
// 
// CoreEngine is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CoreEngine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//  
// You should have received a copy of the GNU General Public License
//  along with Tauron.Application.Common If not, see <http://www.gnu.org/licenses/>.

#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuildContextExtensions.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The build context extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The build context extensions.</summary>
    [PublicAPI]
    public static class BuildContextExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The can cache.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool CanCache([NotNull] this IBuildContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return context.Mode != BuildMode.BuildUpObject || context.Target != null
                   || context.Metadata.Export.ExternalInfo.External
                   && !context.Metadata.Export.ExternalInfo.HandlesLiftime;
        }

        /// <summary>
        ///     The can handle liftime.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool CanHandleLiftime([NotNull] this IBuildContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return !context.Metadata.Export.ExternalInfo.External
                   && !context.Metadata.Export.ExternalInfo.HandlesLiftime;
        }

        /// <summary>
        ///     The can use build up.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool CanUseBuildUp([NotNull] this IBuildContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return !context.Metadata.Export.ExternalInfo.External || context.Metadata.Export.ExternalInfo.CanUseBuildup;
        }

        /// <summary>
        ///     The is build export.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsBuildExport([NotNull] this IBuildContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return context.Mode != BuildMode.BuildUpObject;
        }

        /// <summary>
        ///     The is resolving.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsResolving([NotNull] this IBuildContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return context.Mode == BuildMode.Resolve;
        }

        /// <summary>
        ///     The use internal instantiation.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool UseInternalInstantiation([NotNull] this IBuildContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return !context.Metadata.Export.ExternalInfo.External;
        }

        #endregion
    }
}