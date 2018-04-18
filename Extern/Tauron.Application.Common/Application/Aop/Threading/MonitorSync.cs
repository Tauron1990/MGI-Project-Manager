#region

using System;
using System.Reflection;
using System.Threading;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Tauron.Application.Ioc.LifeTime;

#endregion

namespace Tauron.Application.Aop.Threading
{
    /// <summary>The monitor holder.</summary>
    [PublicAPI]
    public sealed class MonitorHolder : BaseHolder<object>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MonitorHolder" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="MonitorHolder" /> Klasse.
        ///     Initializes a new instance of the <see cref="MonitorHolder" /> class.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        public MonitorHolder([CanBeNull] object value)
            : base(value ?? new object())
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MonitorHolder" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="MonitorHolder" /> Klasse.
        ///     Initializes a new instance of the <see cref="MonitorHolder" /> class.
        /// </summary>
        public MonitorHolder()
            : base(new object())
        {
        }

        #endregion
    }

    /// <summary>The monitor lock attribute.</summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Event)]
    [PublicAPI]
    public sealed class MonitorLockAttribute : ThreadingBaseAspect
    {
        #region Fields

        /// <summary>The _holder.</summary>
        private MonitorHolder _holder;

        #endregion

        #region Methods

        /// <summary>
        ///     The initialize.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="contextName">
        ///     The context name.
        /// </param>
        protected internal override void Initialize(object target, ObjectContext context, string contextName)
        {
            _holder = BaseHolder.GetOrAdd<MonitorHolder, MonitorHolder>(
                                                                        context,
                                                                        () => new MonitorHolder(),
                                                                        HolderName);

            base.Initialize(target, context, contextName);
        }

        /// <summary>
        ///     The intercept impl.
        /// </summary>
        /// <param name="invocation">
        ///     The invocation.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        protected override void Intercept(IInvocation invocation, ObjectContext context)
        {
            var lockTaken = false;

            try
            {
                if (!Monitor.IsEntered(_holder.Value)) Monitor.Enter(_holder.Value, ref lockTaken);

                invocation.Proceed();
            }
            finally
            {
                if (lockTaken) Monitor.Exit(_holder.Value);
            }
        }

        #endregion
    }

    /// <summary>The monitor source attribute.</summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    [PublicAPI]
    public sealed class MonitorSourceAttribute : ContextPropertyAttributeBase
    {
        #region Methods

        /// <summary>
        ///     The register.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="info">
        ///     The info.
        /// </param>
        /// <param name="target">
        ///     The target.
        /// </param>
        protected internal override void Register(ObjectContext context, MemberInfo info, object target)
        {
            context.Register<MonitorHolder, MonitorHolder>(
                                                           new MonitorHolder(info.GetInvokeMember<object>(target))
                                                           {
                                                               Name
                                                                   =
                                                                   HolderName
                                                           });
        }

        #endregion
    }
}