// The file LifetimeTimePolicy.cs is part of Tauron.Application.Common.
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
// <copyright file="LifetimeTimePolicy.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The lifetime time policy.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Tauron.Application.Ioc.LifeTime;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The lifetime time policy.</summary>
    public sealed class LifetimeTimePolicy : IPolicy
    {
        #region Fields

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the lifetime context.</summary>
        /// <value>The lifetime context.</value>
        public ILifetimeContext LifetimeContext { get; set; }

        /// <summary>Gets or sets the liftime type.</summary>
        /// <value>The liftime type.</value>
        public Type LiftimeType { get; set; }

        /// <summary>Gets or sets a value indicating whether share liftime.</summary>
        /// <value>The share liftime.</value>
        public bool ShareLiftime { get; set; }

        #endregion
    }
}