// The file ThreadSharedLifetime.cs is part of Tauron.Application.Common.
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
// <copyright file="ThreadSharedLifetime.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The thread shared lifetime.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;

#endregion

namespace Tauron.Application.Ioc.LifeTime
{
    /// <summary>The thread shared lifetime.</summary>
    public sealed class ThreadSharedLifetime : ILifetimeContext
    {
        #region Fields

        /// <summary>The _objects.</summary>
        private readonly Dictionary<Thread, object> _objects = new Dictionary<Thread, object>();

        #endregion

        #region Public Properties

        /// <summary>Gets a value indicating whether is alive.</summary>
        /// <value>The is alive.</value>
        public bool IsAlive => _objects.ContainsKey(Thread.CurrentThread);

        #endregion

        #region Public Methods and Operators

        /// <summary>The get value.</summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public object GetValue()
        {
            object value;
            return _objects.TryGetValue(Thread.CurrentThread, out value) ? value : null;
        }

        /// <summary>
        ///     The set value.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        public void SetValue(object value)
        {
            _objects[Thread.CurrentThread] = value;
        }

        #endregion
    }
}