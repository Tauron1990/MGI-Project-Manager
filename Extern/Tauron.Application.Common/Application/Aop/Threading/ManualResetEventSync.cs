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
    /// <summary>The manual reset event holder.</summary>
    [PublicAPI]
    public class ManualResetEventHolder : BaseHolder<ManualResetEventSlim>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ManualResetEventHolder" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ManualResetEventHolder" /> Klasse.
        ///     Initializes a new instance of the <see cref="ManualResetEventHolder" /> class.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        public ManualResetEventHolder([CanBeNull] ManualResetEventSlim value)
            : base(value ?? new ManualResetEventSlim())
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ManualResetEventHolder" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ManualResetEventHolder" /> Klasse.
        ///     Initializes a new instance of the <see cref="ManualResetEventHolder" /> class.
        /// </summary>
        public ManualResetEventHolder()
            : base(new ManualResetEventSlim())
        {
        }

        #endregion
    }

    /// <summary>The manual reset event source attribute.</summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class ManualResetEventSourceAttribute : ContextPropertyAttributeBase
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
            context.Register<ManualResetEventHolder, ManualResetEventHolder>(
                new ManualResetEventHolder(
                    info
                        .GetInvokeMember
                        <ManualResetEventSlim>(target))
                {
                    Name = HolderName
                });
        }

        #endregion
    }

    /// <summary>The method invocation position.</summary>
    [PublicAPI]
    public enum MethodInvocationPosition
    {
        /// <summary>The before.</summary>
        Before,

        /// <summary>The after.</summary>
        After
    }

    /// <summary>The manual reset event behavior.</summary>
    [PublicAPI]
    public enum ManualResetEventBehavior
    {
        /// <summary>The set.</summary>
        Set,

        /// <summary>The wait.</summary>
        Wait
    }

    /// <summary>The manual reset event attribute.</summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Event | AttributeTargets.Property)]
    public sealed class ManualResetEventAttribute : ThreadingBaseAspect
    {
        #region Fields

        /// <summary>The _holder.</summary>
        private ManualResetEventHolder _holder;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ManualResetEventAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ManualResetEventAttribute" /> Klasse.
        ///     Initializes a new instance of the <see cref="ManualResetEventAttribute" /> class.
        /// </summary>
        public ManualResetEventAttribute()
        {
            Position = MethodInvocationPosition.Before;
            EventBehavior = ManualResetEventBehavior.Wait;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the event behavior.</summary>
        /// <value>The event behavior.</value>
        public ManualResetEventBehavior EventBehavior { get; set; }

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
            _holder = BaseHolder.GetOrAdd<ManualResetEventHolder, ManualResetEventHolder>(
                context,
                () =>
                    new ManualResetEventHolder(),
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
            if (Position == MethodInvocationPosition.Before)
                if (EventBehavior == ManualResetEventBehavior.Wait)
                {
                    _holder.Value.Wait();
                }
                else
                {
                    _holder.Value.Set();
                    _holder.Value.Reset();
                }

            invocation.Proceed();

            if (EventBehavior == ManualResetEventBehavior.Wait)
            {
                _holder.Value.Wait();
            }
            else
            {
                _holder.Value.Set();
                _holder.Value.Reset();
            }
        }

        #endregion
    }
}