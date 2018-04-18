// The file LifetimeContextAttribute.cs is part of Tauron.Application.Common.
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
// <copyright file="LifetimeContextAttribute.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The lifetime context attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.LifeTime
{
    /// <summary>The lifetime context attribute.</summary>
    [AttributeUsage(AttributeTargets.Class)]
    [PublicAPI]
    public abstract class LifetimeContextAttribute : ExportMetadataBaseAttribute
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LifetimeContextAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="LifetimeContextAttribute" /> Klasse.
        ///     Initializes a new instance of the <see cref="LifetimeContextAttribute" /> class.
        /// </summary>
        /// <param name="lifeTimeType">
        ///     The life time type.
        /// </param>
        protected LifetimeContextAttribute([NotNull] Type lifeTimeType)
            : base(AopConstants.LiftimeMetadataName, null)
        {
            if (lifeTimeType == null) throw new ArgumentNullException(nameof(lifeTimeType));
            LifeTimeType  = lifeTimeType;
            ShareLiftime  = true;
            InternalValue = this;
        }

        #endregion


        #region Public Properties

        /// <summary>Gets the life time type.</summary>
        /// <value>The life time type.</value>
        [NotNull]
        public Type LifeTimeType { get; }

        /// <summary>Gets or sets a value indicating whether share liftime.</summary>
        /// <value>The share liftime.</value>
        public bool ShareLiftime { get; set; }

        #endregion
    }
}