#region

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Tauron.Application.Ioc.LifeTime;

#endregion

namespace Tauron.Application.Aop.Threading
{
    /// <summary>The barrier holder.</summary>
    public class BarrierHolder : BaseHolder<Barrier>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BarrierHolder" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="BarrierHolder" /> Klasse.
        ///     Initializes a new instance of the <see cref="BarrierHolder" /> class.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht",
            Justification = "Used In Upper Scope")]
        public BarrierHolder([CanBeNull] Barrier value)
            : base(value ?? new Barrier(1))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BarrierHolder" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="BarrierHolder" /> Klasse.
        ///     Initializes a new instance of the <see cref="BarrierHolder" /> class.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht",
            Justification = "Used In Upper Scope")]
        public BarrierHolder()
            : base(new Barrier(1))
        {
        }

        #endregion
    }

    /// <summary>The barrier source attribute.</summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    [PublicAPI]
    public sealed class BarrierSourceAttribute : ContextPropertyAttributeBase
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
            context.Register<BarrierHolder, BarrierHolder>(
                new BarrierHolder(info.GetInvokeMember<Barrier>(target))
                {
                    Name
                        =
                        HolderName
                });
        }

        #endregion
    }

    /// <summary>The barrier attribute.</summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Event)]
    [PublicAPI]
    public sealed class BarrierAttribute : ThreadingBaseAspect
    {
        #region Fields

        /// <summary>The _holder.</summary>
        private BarrierHolder _holder;

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the position.</summary>
        /// <value>The position.</value>
        public MethodInvocationPosition Position { get; set; }

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
            _holder = BaseHolder.GetOrAdd<BarrierHolder, BarrierHolder>(
                context,
                () => new BarrierHolder(),
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
            if (Position == MethodInvocationPosition.Before) _holder.Value.SignalAndWait();

            invocation.Proceed();

            if (Position == MethodInvocationPosition.After) _holder.Value.SignalAndWait();
        }

        #endregion
    }
}