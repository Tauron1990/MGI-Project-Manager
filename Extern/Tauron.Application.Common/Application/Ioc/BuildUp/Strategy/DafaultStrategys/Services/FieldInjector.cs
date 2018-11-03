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
        /// <param name="resolverExtensions"></param>
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