// The file ExternalExportInfo.cs is part of Tauron.Application.Common.
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
// <copyright file="ExternalExportInfo.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The external export info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Strategy;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Exports
{
    /// <summary>The external export info.</summary>
    [PublicAPI]
    public sealed class ExternalExportInfo
    {
        #region Fields

        /// <summary>The _create instance.</summary>
        private readonly Func<IBuildContext, ProxyGenerator, object> _createInstance;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExternalExportInfo" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ExternalExportInfo" /> Klasse.
        ///     Initializes a new instance of the <see cref="ExternalExportInfo" /> class.
        /// </summary>
        /// <param name="external">
        ///     The external.
        /// </param>
        /// <param name="handlesLiftime">
        ///     The handles liftime.
        /// </param>
        /// <param name="canUseBuildup">
        ///     The can use buildup.
        /// </param>
        /// <param name="handlesDispose">
        ///     The handles dispose.
        /// </param>
        /// <param name="createInstance">
        ///     The create instance.
        /// </param>
        /// <param name="extenalComponentName">
        ///     The extenal component name.
        /// </param>
        public ExternalExportInfo(
            bool external,
            bool handlesLiftime,
            bool canUseBuildup,
            bool handlesDispose,
            Func<IBuildContext, ProxyGenerator, object> createInstance,
            string extenalComponentName)
        {
            External = external;
            HandlesLiftime = handlesLiftime;
            CanUseBuildup = canUseBuildup;
            HandlesDispose = handlesDispose;
            ExtenalComponentName = extenalComponentName;
            _createInstance = createInstance;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The create.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="service">
        ///     The service.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public object Create(IBuildContext context, ProxyGenerator service)
        {
            return _createInstance(context, service);
        }

        #endregion

        #region Public Properties

        /// <summary>Gets a value indicating whether can use buildup.</summary>
        /// <value>The can use buildup.</value>
        public bool CanUseBuildup { get; private set; }

        /// <summary>Gets the extenal component name.</summary>
        /// <value>The extenal component name.</value>
        public string ExtenalComponentName { get; private set; }

        /// <summary>Gets a value indicating whether external.</summary>
        /// <value>The external.</value>
        public bool External { get; private set; }

        /// <summary>Gets a value indicating whether handles dispose.</summary>
        /// <value>The handles dispose.</value>
        public bool HandlesDispose { get; private set; }

        /// <summary>Gets a value indicating whether handles liftime.</summary>
        /// <value>The handles liftime.</value>
        public bool HandlesLiftime { get; private set; }

        #endregion
    }
}