// The file InjectEventAttribute.cs is part of Tauron.Application.Common.
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
// <copyright file="InjectEventAttribute.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The inject event attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc
{
    /// <summary>The inject event attribute.</summary>
    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Method)]
    [PublicAPI]
    public sealed class InjectEventAttribute : Attribute
    {
        #region Fields

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="InjectEventAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="InjectEventAttribute" /> Klasse.
        ///     Initializes a new instance of the <see cref="InjectEventAttribute" /> class.
        /// </summary>
        /// <param name="topic">
        ///     The topic.
        /// </param>
        public InjectEventAttribute([NotNull] string topic)
        {
            Topic = topic ?? throw new ArgumentNullException(nameof(topic));
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the topic.</summary>
        /// <value>The topic.</value>
        public string Topic { get; }

        #endregion
    }
}