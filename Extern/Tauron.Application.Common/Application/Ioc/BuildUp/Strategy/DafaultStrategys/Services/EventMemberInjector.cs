// The file EventMemberInjector.cs is part of Tauron.Application.Common.
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
// <copyright file="EventMemberInjector.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The event member injector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Reflection;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The event member injector.</summary>
    public class EventMemberInjector : MemberInjector
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="EventMemberInjector" /> Klasse.
        /// </summary>
        /// <param name="metadata">
        ///     The metadata.
        /// </param>
        /// <param name="manager">
        ///     The manager.
        /// </param>
        /// <param name="member">
        ///     The member.
        /// </param>
        public EventMemberInjector([NotNull] ImportMetadata metadata, [NotNull] IEventManager manager, [NotNull] MemberInfo member)
        {
            if (metadata == null) throw new ArgumentNullException(nameof(metadata));
            if (manager == null) throw new ArgumentNullException(nameof(manager));
            if (member == null) throw new ArgumentNullException(nameof(member));
            _metadata = metadata;
            _manager = manager;
            _member = member;
        }

        #endregion

        #region Public Methods and Operators

        public override void Inject(object target, IContainer container, ImportMetadata metadata, IImportInterceptor interceptor, ErrorTracer errorTracer, BuildParameter[] parameters)
        {
            errorTracer.Phase = "EventManager Inject " + metadata.ContractName;

            try
            {
                var eventInfo = _member as EventInfo;
                if (eventInfo != null) _manager.AddPublisher(_metadata.ContractName, eventInfo, target, errorTracer);

                var method = _member as MethodInfo;
                if (method != null) _manager.AddEventHandler(_metadata.ContractName, method, target, errorTracer);
            }
            catch (Exception e)
            {
                errorTracer.Exceptional = true;
                errorTracer.Exception = e;
            }
        }

        #endregion

        #region Fields

        /// <summary>The _manager.</summary>
        private readonly IEventManager _manager;

        /// <summary>The _member.</summary>
        private readonly MemberInfo _member;

        /// <summary>The _metadata.</summary>
        private readonly ImportMetadata _metadata;

        #endregion
    }
}