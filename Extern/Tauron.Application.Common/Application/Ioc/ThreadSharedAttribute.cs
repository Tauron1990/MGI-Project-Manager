// The file ThreadSharedAttribute.cs is part of Tauron.Application.Common.
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
// <copyright file="ThreadSharedAttribute.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The thread shared attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using JetBrains.Annotations;
using Tauron.Application.Ioc.LifeTime;

#endregion

namespace Tauron.Application.Ioc
{
    /// <summary>The thread shared attribute.</summary>
    [AttributeUsage(AttributeTargets.Class)]
    [PublicAPI]
    public sealed class ThreadSharedAttribute : LifetimeContextAttribute
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ThreadSharedAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ThreadSharedAttribute" /> Klasse.
        ///     Initializes a new instance of the <see cref="ThreadSharedAttribute" /> class.
        /// </summary>
        public ThreadSharedAttribute()
            : base(typeof(ThreadSharedLifetime))
        {
        }

        #endregion
    }
}