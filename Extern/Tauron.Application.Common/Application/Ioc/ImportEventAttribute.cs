// The file ImportEventAttribute.cs is part of Tauron.Application.Common.
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
// <copyright file="ImportEventAttribute.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The inject event attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc
{
    /// <summary>The inject event attribute.</summary>
    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Method, AllowMultiple = false)]
    [PublicAPI]
    public sealed class ImportEventAttribute : Attribute
    {
        #region Fields

        private readonly string topic;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImportEventAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ImportEventAttribute" /> Klasse.
        ///     Initializes a new instance of the <see cref="ImportEventAttribute" /> class.
        /// </summary>
        /// <param name="topic">
        ///     The topic.
        /// </param>
        public ImportEventAttribute(string topic)
        {
            Contract.Requires<ArgumentNullException>(topic != null, "topic");

            this.topic = topic;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the topic.</summary>
        /// <value>The topic.</value>
        public string Topic
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);

                return topic;
            }
        }

        #endregion
    }
}