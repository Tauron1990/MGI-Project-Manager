// The file BuildUpException.cs is part of Tauron.Application.Common.
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
// <copyright file="BuildUpException.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   Defines the BuildUpException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc
{
    /// <summary>The build up exception.</summary>
    [Serializable]
    [PublicAPI]
    public sealed class BuildUpException : Exception
    {
        #region Constructors and Destructors

        public BuildUpException([NotNull] ErrorTracer errorTracer)
            : base(errorTracer.Phase + errorTracer.Export, errorTracer.Exception)
        {
            ErrorTracer = errorTracer;
        }

        #endregion

        [NotNull] public ErrorTracer ErrorTracer { get; private set; }
    }
}