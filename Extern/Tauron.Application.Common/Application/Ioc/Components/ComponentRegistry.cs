// The file ComponentRegistry.cs is part of Tauron.Application.Common.
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

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.Components
{
    /// <summary>The component registry.</summary>
    [PublicAPI]
    public sealed class ComponentRegistry : IDisposable
    {
        #region Fields

        /// <summary>The _dictionary.</summary>
        private readonly GroupDictionary<Type, LazyLoad> _dictionary = new GroupDictionary<Type, LazyLoad>();

        #endregion

        /// <summary>The lazy load.</summary>
        private class LazyLoad : IDisposable
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="LazyLoad" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="LazyLoad" /> Klasse.
            ///     Initializes a new instance of the <see cref="LazyLoad" /> class.
            /// </summary>
            /// <param name="implement">
            ///     The implement.
            /// </param>
            /// <param name="registry">
            ///     The registry.
            /// </param>
            /// <param name="instance">
            ///     The instance.
            /// </param>
            public LazyLoad([NotNull] Type implement, [NotNull] ComponentRegistry registry, object instance)
            {
                if (implement == null) throw new ArgumentNullException(nameof(implement));
                if (registry == null) throw new ArgumentNullException(nameof(registry));
                _implement = implement;
                _registry = registry;
                _object = instance;
            }

            #endregion

            #region Public Properties

            /// <summary>Gets the object.</summary>
            /// <value>The object.</value>
            public object Object
            {
                get
                {
                    lock (this)
                    {
                        if (_isInitialized) return _object;

                        if (_object == null) _object = Activator.CreateInstance(_implement);

                        var init = _object as IInitializeable;
                        if (init != null) init.Initialize(_registry);

                        _isInitialized = true;
                    }

                    return _object;
                }
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>The dispose.</summary>
            public void Dispose()
            {
                if (_object is IDisposable disposable) disposable.Dispose();
            }

            #endregion

            public override string ToString()
            {
                return _implement.ToString();
            }

            #region Fields

            /// <summary>The _implement.</summary>
            private readonly Type _implement;

            /// <summary>The _registry.</summary>
            private readonly ComponentRegistry _registry;

            /// <summary>The _is initialized.</summary>
            private bool _isInitialized;

            /// <summary>The _object.</summary>
            private object _object;

            #endregion
        }

        #region Public Methods and Operators

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            foreach (var value in _dictionary.AllValues) value.Dispose();

            _dictionary.Clear();
        }

        /// <summary>The get.</summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <returns>
        ///     The <see cref="TInterface" />.
        /// </returns>
        [System.Diagnostics.Contracts.Pure]
        public TInterface Get<TInterface>() where TInterface : class
        {
            lock (_dictionary)
            {
                var type = typeof(TInterface);
                ICollection<LazyLoad> list;
                if (_dictionary.TryGetValue(type, out list))
                    return (TInterface) list.Single().Object;
            }

            throw new KeyNotFoundException();
        }

        /// <summary>The get all.</summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        public IEnumerable<TInterface> GetAll<TInterface>() where TInterface : class
        {
            lock (_dictionary)
            {
                var type = typeof(TInterface);
                ICollection<LazyLoad> list;
                if (!_dictionary.TryGetValue(type, out list)) yield break;

                foreach (var lazyLoad in list) yield return (TInterface) lazyLoad.Object;
            }
        }

        /// <summary>The register.</summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TImplement"></typeparam>
        public void Register<TInterface, TImplement>() where TImplement : TInterface, new()
        {
            lock (_dictionary)
            {
                _dictionary[typeof(TInterface)].Add(new LazyLoad(typeof(TImplement), this, null));
            }
        }

        public void Register<TInterface, TImplement>(bool single) where TImplement : TInterface, new()
        {
            lock (_dictionary)
            {
                if (single)
                {
                    var temp = _dictionary[typeof(TInterface)];
                    temp.Clear();
                    temp.Add(new LazyLoad(typeof(TImplement), this, null));
                    return;
                }

                _dictionary[typeof(TInterface)].Add(new LazyLoad(typeof(TImplement), this, null));
            }
        }

        /// <summary>
        ///     The register.
        /// </summary>
        /// <param name="instance">
        ///     The instance.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <typeparam name="T1">
        /// </typeparam>
        public void Register<T, T1>([NotNull] T1 instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            lock (_dictionary)
            {
                _dictionary[typeof(T)].Add(new LazyLoad(typeof(T1), this, instance));
            }
        }

        public void Register<T, T1>([NotNull] T1 instance, bool isSingle)
            where T1 : T
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            lock (_dictionary)
            {
                if (isSingle)
                {
                    var temp = _dictionary[typeof(T)];
                    temp.Clear();
                    temp.Add(new LazyLoad(typeof(T1), this, instance));
                }

                _dictionary[typeof(T)].Add(new LazyLoad(typeof(T1), this, instance));
            }
        }

        #endregion
    }
}