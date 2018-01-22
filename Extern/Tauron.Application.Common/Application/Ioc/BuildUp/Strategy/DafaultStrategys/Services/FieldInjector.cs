// The file FieldInjector.cs is part of Tauron.Application.Common.
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

using System;
using System.Reflection;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The field injector.</summary>
    public class FieldInjector : Injectorbase<FieldInfo>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="FieldInjector" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="FieldInjector" /> Klasse.
        ///     Initializes a new instance of the <see cref="FieldInjector" /> class.
        /// </summary>
        /// <param name="metadataFactory">
        ///     The metadata factory.
        /// </param>
        /// <param name="member">
        ///     The member.
        /// </param>
        public FieldInjector([NotNull] IMetadataFactory metadataFactory, [NotNull] FieldInfo member, [NotNull] [ItemNotNull] IResolverExtension[] resolverExtensions)
            : base(metadataFactory, member, resolverExtensions)
        {
            if (metadataFactory == null) throw new ArgumentNullException(nameof(metadataFactory));
            if (member == null) throw new ArgumentNullException(nameof(member));
            if (resolverExtensions == null) throw new ArgumentNullException(nameof(resolverExtensions));
        }

        #endregion

        #region Properties

        /// <summary>The get member type.</summary>
        /// <returns>
        ///     The <see cref="Type" />.
        /// </returns>
        /// <value>The member type.</value>
        protected override Type MemberType => Member.FieldType;

        #endregion

        #region Methods

        /// <summary>
        ///     The inject.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        protected override void Inject(object target, object value)
        {
            Member.SetValue(target, value);
        }

        #endregion
    }
}