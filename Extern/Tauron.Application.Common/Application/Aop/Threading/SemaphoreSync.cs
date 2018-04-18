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
    /// <summary>The semaphore holder.</summary>
    public class SemaphoreHolder : BaseHolder<SemaphoreSlim>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SemaphoreHolder" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="SemaphoreHolder" /> Klasse.
        ///     Initializes a new instance of the <see cref="SemaphoreHolder" /> class.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        public SemaphoreHolder([CanBeNull] SemaphoreSlim value)
            : base(value ?? new SemaphoreSlim(1))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SemaphoreHolder" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="SemaphoreHolder" /> Klasse.
        ///     Initializes a new instance of the <see cref="SemaphoreHolder" /> class.
        /// </summary>
        public SemaphoreHolder()
            : base(new SemaphoreSlim(1))
        {
        }

        #endregion
    }

    /// <summary>The semaphore source attribute.</summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    [PublicAPI]
    public sealed class SemaphoreSourceAttribute : ContextPropertyAttributeBase
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
            context.Register<SemaphoreHolder, SemaphoreHolder>(
                                                               new SemaphoreHolder(
                                                                                   info.GetInvokeMember<SemaphoreSlim>(target))
                                                               {
                                                                   Name
                                                                       =
                                                                       HolderName
                                                               });
        }

        #endregion
    }

    /// <summary>The semaphore attribute.</summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Event, AllowMultiple = true)]
    public sealed class SemaphoreAttribute : ThreadingBaseAspect
    {
        #region Fields

        /// <summary>The _holder.</summary>
        private SemaphoreHolder _holder;

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
            _holder = BaseHolder.GetOrAdd<SemaphoreHolder, SemaphoreHolder>(
                                                                            context,
                                                                            () => new SemaphoreHolder(),
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
        protected override void Intercept([NotNull] IInvocation invocation, [NotNull] ObjectContext context)
        {
            _holder.Value.Wait();
            try
            {
                invocation.Proceed();
            }
            finally
            {
                _holder.Value.Release();
            }
        }

        #endregion
    }
}