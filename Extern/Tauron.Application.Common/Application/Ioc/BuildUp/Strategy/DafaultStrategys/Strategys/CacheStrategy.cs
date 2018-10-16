// The file CacheStrategy.cs is part of Tauron.Application.Common.
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
// <copyright file="CacheStrategy.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The cache strategy.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Tauron.Application.Ioc.Components;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The cache strategy.</summary>
    public class CacheStrategy : StrategyBase
    {
        #region Fields

        /// <summary>The _cache.</summary>
        private ICache _cache;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The initialize.
        /// </summary>
        /// <param name="components">
        ///     The components.
        /// </param>
        public override void Initialize(ComponentRegistry components)
        {
            _cache = components.Get<ICache>();
        }

        /// <summary>
        ///     The on perpare.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        public override void OnPerpare(IBuildContext context)
        {
            if (!context.IsBuildExport()) return;

            context.ErrorTracer.Phase = "Reciving Build (" + context.Metadata + ") from Cache";

            var life = _cache.GetContext(context.Metadata);
            if (life == null) return;

            var value = life.GetValue();
            if (value == null) return;

            context.Target = value;
            context.BuildCompled = true;
        }

        /// <summary>
        ///     The on post build.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        public override void OnPostBuild(IBuildContext context)
        {
            if (!context.CanCache()) return;

            context.ErrorTracer.Phase = "Saving Build (" + context.Metadata + ") to Cache";

            var policy = context.Policys.Get<LifetimeTimePolicy>();

            if (policy == null) return;

            _cache.Add(policy.LifetimeContext, context.Metadata, policy.ShareLiftime);
        }

        #endregion
    }
}