// The file IBuildContext.cs is part of Tauron.Application.Common.
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
// <copyright file="IBuildContext.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The BuildContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy
{
    /// <summary>The BuildContext interface.</summary>
    [PublicAPI]
    public interface IBuildContext
    {
        #region Public Properties

        IResolverExtension[] ResolverExtensions { get; }

        /// <summary>Gets or sets a value indicating whether build compled.</summary>
        /// <value>The build compled.</value>
        bool BuildCompled { get; set; }

        /// <summary>Gets the container.</summary>
        /// <value>The container.</value>
        [NotNull]
        IContainer Container { get; }

        /// <summary>Gets or sets the export type.</summary>
        /// <value>The export type.</value>
        [NotNull]
        Type ExportType { get; set; }

        /// <summary>Gets the metadata.</summary>
        /// <value>The metadata.</value>
        [NotNull]
        ExportMetadata Metadata { get; }

        /// <summary>Gets the mode.</summary>
        /// <value>The mode.</value>
        BuildMode Mode { get; }

        /// <summary>Gets the policys.</summary>
        /// <value>The policys.</value>
        [NotNull]
        PolicyList Policys { get; }

        /// <summary>Gets or sets the target.</summary>
        /// <value>The target.</value>
        [CanBeNull]
        object Target { get; set; }

        [NotNull] ErrorTracer ErrorTracer { get; }

        [CanBeNull] BuildParameter[] Parameters { get; }

        #endregion
    }
}