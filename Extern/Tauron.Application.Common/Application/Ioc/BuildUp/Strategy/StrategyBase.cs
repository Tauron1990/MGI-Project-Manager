// The file StrategyBase.cs is part of Tauron.Application.Common.
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
// <copyright file="StrategyBase.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The strategy base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using JetBrains.Annotations;
using Tauron.Application.Ioc.Components;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy
{
    /// <summary>The strategy base.</summary>
    public abstract class StrategyBase : IStrategy
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The initialize.
        /// </summary>
        /// <param name="components">
        ///     The components.
        /// </param>
        public virtual void Initialize([NotNull] ComponentRegistry components)
        {
        }

        /// <summary>
        ///     The on build.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        public virtual void OnBuild(IBuildContext context)
        {
        }

        /// <summary>
        ///     The on create instance.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        public virtual void OnCreateInstance(IBuildContext context)
        {
        }

        /// <summary>
        ///     The on perpare.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        public virtual void OnPerpare(IBuildContext context)
        {
        }

        /// <summary>
        ///     The on post build.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        public virtual void OnPostBuild(IBuildContext context)
        {
        }

        #endregion
    }
}