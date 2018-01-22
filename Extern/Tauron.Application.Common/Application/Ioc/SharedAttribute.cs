// The file SharedAttribute.cs is part of Tauron.Application.Common.
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
// <copyright file="SharedAttribute.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The shared attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using JetBrains.Annotations;
using Tauron.Application.Ioc.LifeTime;

#endregion

namespace Tauron.Application.Ioc
{
    /// <summary>The shared attribute.</summary>
    [AttributeUsage(AttributeTargets.Class)]
    [PublicAPI]
    public sealed class SharedAttribute : LifetimeContextAttribute
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SharedAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="SharedAttribute" /> Klasse.
        ///     Initializes a new instance of the <see cref="SharedAttribute" /> class.
        /// </summary>
        public SharedAttribute()
            : base(typeof(SharedLifetime))
        {
        }

        #endregion
    }
}