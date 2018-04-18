// The file ObjectContext.cs is part of Tauron.Application.Common.
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
// <copyright file="ObjectContext.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The object context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Tauron.Application.Ioc.Components;

#endregion

namespace Tauron.Application.Ioc.LifeTime
{
    /// <summary>The object context.</summary>
    [PublicAPI]
    public sealed class ObjectContext : IDisposable
    {
        #region Fields

        /// <summary>The _registry.</summary>
        private readonly ComponentRegistry _registry;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObjectContext" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ObjectContext" /> Klasse.
        ///     Initializes a new instance of the <see cref="ObjectContext" /> class.
        /// </summary>
        public ObjectContext()
        {
            _registry = new ComponentRegistry();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            _registry.Dispose();
        }

        /// <summary>The dispose all.</summary>
        public void DisposeAll()
        {
            lock (_registry) _registry.Dispose();
        }

        /// <summary>The get.</summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <returns>
        ///     The <see cref="TInterface" />.
        /// </returns>
        public TInterface Get<TInterface>() where TInterface : class
        {
            return _registry.Get<TInterface>();
        }

        public IEnumerable<TInterface> GetAll<TInterface>() where TInterface : class
        {
            return _registry.GetAll<TInterface>();
        }

        /// <summary>The register.</summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TImplement"></typeparam>
        public void Register<TInterface, TImplement>() where TImplement : TInterface, new()
        {
            _registry.Register<TInterface, TImplement>();
        }

        /// <summary>
        ///     The register.
        /// </summary>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <typeparam name="TInterface">
        /// </typeparam>
        /// <typeparam name="TImplement">
        /// </typeparam>
        public void Register<TInterface, TImplement>([NotNull] TImplement instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            _registry.Register<TInterface, TImplement>(instance);
        }

        #endregion
    }
}