// The file InterceptAttribute.cs is part of Tauron.Application.Common.
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
// <copyright file="InterceptAttribute.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The intercept attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Castle.DynamicProxy;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc
{
    /// <summary>The intercept attribute.</summary>
    [AttributeUsage(AttributeTargets.Class)]
    [PublicAPI]
    public class InterceptAttribute : ExportMetadataBaseAttribute
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="InterceptAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="InterceptAttribute" /> Klasse.
        ///     Initializes a new instance of the <see cref="InterceptAttribute" /> class.
        /// </summary>
        public InterceptAttribute()
            : base(AopConstants.InterceptMetadataName, null)
        {
            InternalValue = this;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The create.</summary>
        /// <returns>
        ///     The <see cref="IInterceptor" />.
        /// </returns>
        [CanBeNull]
        public virtual IInterceptor Create()
        {
            return null;
        }

        /// <summary>
        ///     The initialize.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        public virtual void Initialize([NotNull] object target)
        {
        }

        #endregion
    }
}