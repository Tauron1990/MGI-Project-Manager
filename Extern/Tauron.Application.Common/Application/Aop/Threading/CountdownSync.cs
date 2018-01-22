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
    /// <summary>The countdown event holder.</summary>
    public sealed class CountdownEventHolder : BaseHolder<CountdownEvent>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CountdownEventHolder" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="CountdownEventHolder" /> Klasse.
        ///     Initializes a new instance of the <see cref="CountdownEventHolder" /> class.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        public CountdownEventHolder([CanBeNull] CountdownEvent value)
            : base(value ?? new CountdownEvent(1))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CountdownEventHolder" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="CountdownEventHolder" /> Klasse.
        ///     Initializes a new instance of the <see cref="CountdownEventHolder" /> class.
        /// </summary>
        public CountdownEventHolder()
            : base(new CountdownEvent(1))
        {
        }

        #endregion
    }

    /// <summary>The countdown event source attribute.</summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class CountdownEventSourceAttribute : ContextPropertyAttributeBase
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
            context.Register<CountdownEventHolder, CountdownEventHolder>(
                new CountdownEventHolder(
                    info.GetInvokeMember<CountdownEvent>(target))
                {
                    Name
                        =
                        HolderName
                });
        }

        #endregion
    }

    /// <summary>The countdown event action.</summary>
    public enum CountdownEventAction
    {
        /// <summary>The add.</summary>
        Add,

        /// <summary>The signal.</summary>
        Signal,

        /// <summary>The wait.</summary>
        Wait
    }

    /// <summary>The countdown event attribute.</summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, AllowMultiple = true)]
    public sealed class CountdownEventAttribute : ThreadingBaseAspect
    {
        #region Fields

        /// <summary>The _holder.</summary>
        private CountdownEventHolder _holder;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CountdownEventAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="CountdownEventAttribute" /> Klasse.
        ///     Initializes a new instance of the <see cref="CountdownEventAttribute" /> class.
        /// </summary>
        public CountdownEventAttribute()
        {
            Count = 1;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the count.</summary>
        /// <value>The count.</value>
        public int Count { get; set; }

        /// <summary>Gets or sets the event action.</summary>
        /// <value>The event action.</value>
        public CountdownEventAction EventAction { get; set; }

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
            _holder = BaseHolder.GetOrAdd<CountdownEventHolder, CountdownEventHolder>(
                context,
                () => new CountdownEventHolder(),
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
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        protected override void Intercept(IInvocation invocation, ObjectContext context)
        {
            switch (EventAction)
            {
                case CountdownEventAction.Add:
                    _holder.Value.AddCount(Count);
                    break;
                case CountdownEventAction.Signal:
                    _holder.Value.Signal(Count);
                    break;
                case CountdownEventAction.Wait:
                    _holder.Value.Wait();
                    break;
            }

            invocation.Proceed();
        }

        #endregion
    }
}