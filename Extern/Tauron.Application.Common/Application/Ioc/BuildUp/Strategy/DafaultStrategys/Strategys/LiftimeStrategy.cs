// The file LiftimeStrategy.cs is part of Tauron.Application.Common.
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
// <copyright file="LiftimeStrategy.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The liftime strategy.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Tauron.Application.Ioc.LifeTime;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The liftime strategy.</summary>
    public sealed class LiftimeStrategy : StrategyBase
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The on perpare.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        public override void OnPerpare(IBuildContext context)
        {
            if (!context.CanHandleLiftime()) return;
            context.ErrorTracer.Phase = "Reciving Liftime Informations for " + context.Metadata;

            context.Policys.Add(
                new LifetimeTimePolicy
                {
                    LiftimeType = context.Metadata.Lifetime,
                    ShareLiftime = context.Metadata.Export.ShareLifetime
                });
        }

        /// <summary>
        ///     The on post build.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        public override void OnPostBuild(IBuildContext context)
        {
            var policy = context.Policys.Get<LifetimeTimePolicy>();
            if (policy == null) return;

            context.ErrorTracer.Phase = "Setting up Liftime for " + context.Metadata;

            policy.LifetimeContext = (ILifetimeContext) Activator.CreateInstance(policy.LiftimeType);
            policy.LifetimeContext.SetValue(context.Target);
        }

        #endregion
    }
}