// The file InjectAttribute.cs is part of Tauron.Application.Common.
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
// <copyright file="InjectAttribute.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The inject attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc
{
    /// <summary>The inject attribute.</summary>
    [AttributeUsage(
        AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Parameter
        | AttributeTargets.Property)]
    [PublicAPI]
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    public class InjectAttribute : Attribute
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="InjectAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="InjectAttribute" /> Klasse.
        ///     Initializes a new instance of the <see cref="InjectAttribute" /> class.
        /// </summary>
        public InjectAttribute()
        {
            ContractName = null;
            Optional = false;
        }

        public InjectAttribute(string contractName)
            : this()
        {
            ContractName = contractName;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="InjectAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="InjectAttribute" /> Klasse.
        ///     Initializes a new instance of the <see cref="InjectAttribute" /> class.
        /// </summary>
        /// <param name="interface">
        ///     The interface.
        /// </param>
        public InjectAttribute(Type @interface)
            : this()
        {
            Interface = @interface;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the contract name.</summary>
        /// <value>The contract name.</value>
        public string ContractName { get; set; }

        /// <summary>Gets or sets the interface.</summary>
        /// <value>The interface.</value>
        public Type Interface { get; private set; }

        /// <summary>Gets or sets a value indicating whether optional.</summary>
        /// <value>The optional.</value>
        public bool Optional { get; set; }

        [NotNull]
        public virtual Dictionary<string, object> CreateMetadata()
        {
            return new Dictionary<string, object>();
        }

        #endregion
    }
}