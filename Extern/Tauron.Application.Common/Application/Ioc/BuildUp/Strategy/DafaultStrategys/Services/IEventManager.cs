// The file IEventManager.cs is part of Tauron.Application.Common.
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
// <copyright file="IEventManager.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The EventManager interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Reflection;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The EventManager interface.</summary>
    [PublicAPI]
    public interface IEventManager
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The add handler.
        /// </summary>
        /// <param name="topic">
        ///     The topic.
        /// </param>
        /// <param name="handler">
        ///     The handler.
        /// </param>
        void AddEventHandler(string topic, Delegate handler, ErrorTracer errorTracer);

        /// <summary>
        ///     The add handler.
        /// </summary>
        /// <param name="topic">
        ///     The topic.
        /// </param>
        /// <param name="handler">
        ///     The handler.
        /// </param>
        /// <param name="target">
        ///     The target.
        /// </param>
        void AddEventHandler(string topic, MethodInfo handler, object target, ErrorTracer errorTracer);

        /// <summary>
        ///     The add publisher.
        /// </summary>
        /// <param name="topic">
        ///     The topic.
        /// </param>
        /// <param name="eventInfo">
        ///     The event info.
        /// </param>
        /// <param name="publisher">
        ///     The publisher.
        /// </param>
        void AddPublisher(string topic, EventInfo eventInfo, object publisher, ErrorTracer errorTracer);

        #endregion
    }
}