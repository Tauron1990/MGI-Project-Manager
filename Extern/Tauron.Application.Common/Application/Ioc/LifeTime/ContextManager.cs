// The file ContextManager.cs is part of Tauron.Application.Common.
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
// <copyright file="ContextManager.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The context manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.LifeTime
{
    /// <summary>The context manager.</summary>
    [PublicAPI]
    public static class ContextManager
    {
        /// <summary>The weak context.</summary>
        private class WeakContext : IWeakReference
        {
            #region Fields

            /// <summary>The _holder.</summary>
            private readonly WeakReference _holder;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="WeakContext" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="WeakContext" /> Klasse.
            ///     Initializes a new instance of the <see cref="WeakContext" /> class.
            /// </summary>
            /// <param name="owner">
            ///     The owner.
            /// </param>
            public WeakContext(object owner)
            {
                _holder = new WeakReference(owner);
            }

            #endregion

            #region Public Properties

            /// <summary>Gets or sets the context.</summary>
            /// <value>The context.</value>
            public ObjectContext Context { get; set; }

            /// <summary>Gets the owner.</summary>
            /// <value>The owner.</value>
            public object Owner => _holder.Target;

            /// <summary>Gets a value indicating whether is alive.</summary>
            /// <value>The is alive.</value>
            public bool IsAlive => _holder.IsAlive;

            #endregion
        }

        #region Static Fields

        /// <summary>The _aspect contexts.</summary>
        private static readonly Dictionary<string, WeakContext> AspectContexts = Initialize();

        /// <summary>The _weak contexts.</summary>
        private static WeakReferenceCollection<WeakContext> _weakContexts;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The find context.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="contextName">
        ///     The context name.
        /// </param>
        /// <returns>
        ///     The <see cref="ObjectContext" />.
        /// </returns>
        public static ObjectContext FindContext(object target, string contextName)
        {
            if (contextName != null)
            {
                var weakHolder = AspectContexts[contextName];
                var context = weakHolder.Context;
                return context;
            }

            var holder = target as IContextHolder;
            if (holder != null) return holder.Context;

            var temp = _weakContexts.FirstOrDefault(con => ReferenceEquals(target, con.Owner));
            if (temp == null) throw new InvalidOperationException();

            return temp.Context;
        }

        /// <summary>
        ///     The get context.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="owner">
        ///     The owner.
        /// </param>
        /// <returns>
        ///     The <see cref="ObjectContext" />.
        /// </returns>
        public static ObjectContext GetContext(string name, [NotNull] object owner)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            WeakContext context;
            if (AspectContexts.TryGetValue(name, out context))
                return context.Context;

            var tempContext = new ObjectContext();
            AddContext(name, tempContext, owner);

            return tempContext;
        }

        /// <summary>
        ///     The get context.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <returns>
        ///     The <see cref="ObjectContext" />.
        /// </returns>
        public static ObjectContext GetContext([NotNull] object target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            var context = new ObjectContext();
            _weakContexts.Add(new WeakContext(target) {Context = context});
            return context;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The add context.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="owner">
        ///     The owner.
        /// </param>
        private static void AddContext([NotNull] string name, ObjectContext context, object owner)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            AspectContexts[name] = new WeakContext(owner) {Context = context};
        }

        /// <summary>The clean contexts.</summary>
        private static void CleanContexts()
        {
            lock (AspectContexts)
            {
                var reference =
                    AspectContexts.Where(pair => !pair.Value.IsAlive).Select(pair => pair.Key).ToArray();
                foreach (var equalableWeakReference in reference) AspectContexts.Remove(equalableWeakReference);
            }
        }

        private static Dictionary<string, WeakContext> Initialize()
        {
            WeakCleanUp.RegisterAction(CleanContexts);
            _weakContexts = new WeakReferenceCollection<WeakContext>();
            return new Dictionary<string, WeakContext>();
        }

        #endregion
    }
}