#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Tauron.Application.Ioc.BuildUp.Exports;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    [PublicAPI]
    public class ParameterMemberInfo : MemberInfo
    {
        public ParameterMemberInfo([NotNull] ParameterInfo parameterInfo)
        {
            ParameterInfo = parameterInfo;
        }

        [NotNull] public ParameterInfo ParameterInfo { get; private set; }

        public override MemberTypes MemberType => 0;

        public override string Name => ParameterInfo.Name;

        public override Type DeclaringType => ParameterInfo.Member.DeclaringType;

        public override Type ReflectedType => ParameterInfo.Member.ReflectedType;

        public override int MetadataToken => ParameterInfo.MetadataToken;

        public override Module Module => ParameterInfo.Member.Module;

        public override object[] GetCustomAttributes(bool inherit)
        {
            return ParameterInfo.GetCustomAttributes(inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return ParameterInfo.IsDefined(attributeType, inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return ParameterInfo.GetCustomAttributes(attributeType, inherit);
        }

        [NotNull]
        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return ParameterInfo.GetCustomAttributesData();
        }
    }

    /// <summary>The method injector.</summary>
    public class MethodInjector : MemberInjector
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MethodInjector" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="MethodInjector" /> Klasse.
        ///     Initializes a new instance of the <see cref="MethodInjector" /> class.
        /// </summary>
        /// <param name="method">
        ///     The method.
        /// </param>
        /// <param name="metadataFactory">
        ///     The metadata factory.
        /// </param>
        /// <param name="eventManager">
        ///     The event manager.
        /// </param>
        /// <param name="resolverExtensions"></param>
        public MethodInjector([NotNull] MethodInfo method, [NotNull] IMetadataFactory metadataFactory,
            [NotNull] IEventManager eventManager,
            [NotNull] [ItemNotNull] IResolverExtension[] resolverExtensions)
        {
            _method = method ?? throw new ArgumentNullException(nameof(method));
            _metadataFactory = metadataFactory ?? throw new ArgumentNullException(nameof(metadataFactory));
            _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            _resolverExtensions = resolverExtensions ?? throw new ArgumentNullException(nameof(resolverExtensions));
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The inject.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="container">
        ///     The container.
        /// </param>
        /// <param name="metadata">
        ///     The metadata.
        /// </param>
        /// <param name="interceptor"></param>
        /// <param name="errorTracer"></param>
        /// <param name="parameters"></param>
        public override void Inject(object target, IContainer container, ImportMetadata metadata,
            IImportInterceptor interceptor, ErrorTracer errorTracer, BuildParameter[] parameters)
        {
            if (metadata.Metadata != null)
                if (metadata.Metadata.TryGetValue(AopConstants.EventMetodMetadataName, out var obj))
                    if ((bool) obj)
                    {
                        var topic = (string) metadata.Metadata[AopConstants.EventTopicMetadataName];
                        _eventManager.AddEventHandler(topic, _method, target, errorTracer);
                        return;
                    }

            var parms = _method.GetParameters();
            var args = new List<object>();

            foreach (var parameterInfo in parms.Select(p => new ParameterMemberInfo(p)))
                new ParameterHelper(_metadataFactory, parameterInfo, args, _resolverExtensions).Inject(target,
                    container, metadata, interceptor, errorTracer, parameters);

            _method.Invoke(target, args.ToArray());
        }

        #endregion

        /// <summary>The parameter helper.</summary>
        private class ParameterHelper : Injectorbase<ParameterMemberInfo>
        {
            #region Fields

            /// <summary>The _parameters.</summary>
            private readonly List<object> _parameters;

            #endregion

            #region Constructors and Destructors

            public ParameterHelper([NotNull] IMetadataFactory metadataFactory, [NotNull] ParameterMemberInfo parameter,
                [NotNull] List<object> parameters, [NotNull] IResolverExtension[] resolverExtensions)
                : base(metadataFactory, parameter, resolverExtensions)
            {
                if (metadataFactory == null) throw new ArgumentNullException(nameof(metadataFactory));
                if (parameter == null) throw new ArgumentNullException(nameof(parameter));
                if (resolverExtensions == null) throw new ArgumentNullException(nameof(resolverExtensions));
                _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            }

            #endregion

            #region Properties

            /// <summary>The get member type.</summary>
            /// <returns>
            ///     The <see cref="Type" />.
            /// </returns>
            /// <value>The member type.</value>
            protected override Type MemberType => Member.ParameterInfo.ParameterType;

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
                _parameters.Add(value);
            }

            #endregion
        }

        #region Fields

        /// <summary>The _event manager.</summary>
        private readonly IEventManager _eventManager;

        private readonly IResolverExtension[] _resolverExtensions;

        /// <summary>The _metadata factory.</summary>
        private readonly IMetadataFactory _metadataFactory;

        /// <summary>The _method.</summary>
        private readonly MethodInfo _method;

        #endregion
    }
}