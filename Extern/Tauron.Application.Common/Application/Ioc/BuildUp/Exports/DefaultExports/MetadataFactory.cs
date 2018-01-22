// The file MetadataFactory.cs is part of Tauron.Application.Common.
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
// <copyright file="MetadataFactory.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The metadata base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Castle.DynamicProxy;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Exports.DefaultExports
{
    /// <summary>The metadata base.</summary>
    public abstract class MetadataBase
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MetadataBase" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="MetadataBase" /> Klasse.
        ///     Initializes a new instance of the <see cref="MetadataBase" /> class.
        /// </summary>
        /// <param name="metadata">
        ///     The metadata.
        /// </param>
        protected MetadataBase([NotNull] IDictionary<string, object> metadata)
        {
            if (metadata == null) throw new ArgumentNullException(nameof(metadata));
            Metadata = new Dictionary<string, object>(metadata);
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the metadata.</summary>
        /// <value>The metadata.</value>
        public Dictionary<string, object> Metadata { get; }

        #endregion

        #region Fields

        #endregion
    }

    /// <summary>The metadata interceptor.</summary>
    [DebuggerNonUserCode]
    public sealed class MetadataInterceptor : IInterceptor
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The intercept.
        /// </summary>
        /// <param name="invocation">
        ///     The invocation.
        /// </param>
        public void Intercept(IInvocation invocation)
        {
            var name = invocation.Method.Name.Remove(0, 4);
            var metadata = (MetadataBase) invocation.Proxy;

            metadata.Metadata.TryGetValue(name, out var value);
            invocation.ReturnValue = value;
        }

        #endregion
    }

    /// <summary>The metadata factory.</summary>
    [PublicAPI]
    public class MetadataFactory : IMetadataFactory
    {
        #region Fields

        /// <summary>The _generator.</summary>
        private readonly ProxyGenerator _generator = new ProxyGenerator();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The create metadata.
        /// </summary>
        /// <param name="interfaceType">
        ///     The interface.
        /// </param>
        /// <param name="metadata">
        ///     The metadata.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public object CreateMetadata(Type interfaceType, IDictionary<string, object> metadata)
        {
            lock (this)
            {
                return _generator.CreateClassProxy(
                    typeof(MetadataBase),
                    new[] {interfaceType},
                    ProxyGenerationOptions.Default,
                    new object[] {metadata},
                    new MetadataInterceptor());
            }
        }

        #endregion
    }
}