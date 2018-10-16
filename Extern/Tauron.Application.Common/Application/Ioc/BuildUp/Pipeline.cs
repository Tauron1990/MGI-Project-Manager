// The file Pipeline.cs is part of Tauron.Application.Common.
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
// <copyright file="Pipeline.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The pipeline.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Strategy;
using Tauron.Application.Ioc.Components;

#endregion

namespace Tauron.Application.Ioc.BuildUp
{
    /// <summary>The pipeline.</summary>
    public sealed class Pipeline
    {
        #region Fields

        /// <summary>The registry.</summary>
        private readonly ComponentRegistry registry;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Pipeline" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="Pipeline" /> Klasse.
        ///     Initializes a new instance of the <see cref="Pipeline" /> class.
        /// </summary>
        /// <param name="registry">
        ///     The registry.
        /// </param>
        public Pipeline([NotNull] ComponentRegistry registry)
        {
            if (registry == null) throw new ArgumentNullException(nameof(registry));
            this.registry = registry;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The build.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        public void Build(IBuildContext context)
        {
            try
            {
                IEnumerable<IStrategy> strategies = registry.GetAll<IStrategy>().ToArray();
                if (Invoke(strategies, strategy => strategy.OnPerpare(context), context)) return;

                if (Invoke(strategies, strategy => strategy.OnCreateInstance(context), context)) return;

                if (Invoke(strategies.Reverse(), strategy => strategy.OnBuild(context), context)) return;

                Invoke(strategies.Reverse(), strategy => strategy.OnPostBuild(context), context);
            }
            catch (Exception e)
            {
                context.ErrorTracer.Exceptional = true;
                context.ErrorTracer.Exception = e;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The invoke.
        /// </summary>
        /// <param name="strategies">
        ///     The strategies.
        /// </param>
        /// <param name="invoker">
        ///     The invoker.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private static bool Invoke([NotNull] IEnumerable<IStrategy> strategies, [NotNull] Action<IStrategy> invoker,
            [NotNull] IBuildContext context)
        {
            if (strategies == null) throw new ArgumentNullException(nameof(strategies));
            if (invoker == null) throw new ArgumentNullException(nameof(invoker));
            if (context == null) throw new ArgumentNullException(nameof(context));
            foreach (var strategy in strategies)
            {
                if (context.BuildCompled || context.ErrorTracer.Exceptional) return true;

                invoker(strategy);
            }

            return false;
        }

        #endregion
    }
}