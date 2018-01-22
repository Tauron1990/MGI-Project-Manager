// The file ObjectContextPolicy.cs is part of Tauron.Application.Common.
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
// <copyright file="ObjectContextPolicy.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The object context policy.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The object context policy.</summary>
    public class ObjectContextPolicy : IPolicy
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObjectContextPolicy" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ObjectContextPolicy" /> Klasse.
        ///     Initializes a new instance of the <see cref="ObjectContextPolicy" /> class.
        /// </summary>
        public ObjectContextPolicy()
        {
            ContextPropertys = new List<Tuple<ObjectContextPropertyAttribute, MemberInfo>>();
        }

        #endregion

        #region Fields

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the context name.</summary>
        /// <value>The context name.</value>
        public string ContextName { get; set; }

        /// <summary>Gets or sets the context propertys.</summary>
        /// <value>The context propertys.</value>
        public List<Tuple<ObjectContextPropertyAttribute, MemberInfo>> ContextPropertys { get; }

        #endregion
    }
}