// The file SharedLifetime.cs is part of Tauron.Application.Common.
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
// <copyright file="SharedLifetime.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The shared lifetime.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.LifeTime
{
    /// <summary>The shared lifetime.</summary>
    [PublicAPI]
    public sealed class SharedLifetime : MarshalByRefObject, ILifetimeContext
    {
        #region Fields

        /// <summary>The _value.</summary>
        private object _value;

        #endregion

        #region Public Properties

        /// <summary>Gets a value indicating whether is alive.</summary>
        /// <value>The is alive.</value>
        public bool IsAlive => _value != null;

        #endregion

        #region Public Methods and Operators

        /// <summary>The get value.</summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public object GetValue()
        {
            return _value;
        }

        /// <summary>
        ///     The set value.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        public void SetValue(object value)
        {
            _value = value;
        }

        #endregion
    }
}