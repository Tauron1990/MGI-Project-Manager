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
    /// <summary>The reader writer lock holder.</summary>
    public class ReaderWriterLockHolder : BaseHolder<ReaderWriterLockSlim>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReaderWriterLockHolder" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ReaderWriterLockHolder" /> Klasse.
        ///     Initializes a new instance of the <see cref="ReaderWriterLockHolder" /> class.
        /// </summary>
        /// <param name="lockSlim">
        ///     The lock slim.
        /// </param>
        public ReaderWriterLockHolder([CanBeNull] ReaderWriterLockSlim lockSlim)
            : base(lockSlim ?? new ReaderWriterLockSlim())
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReaderWriterLockHolder" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ReaderWriterLockHolder" /> Klasse.
        ///     Initializes a new instance of the <see cref="ReaderWriterLockHolder" /> class.
        /// </summary>
        public ReaderWriterLockHolder()
            : base(new ReaderWriterLockSlim())
        {
        }

        #endregion
    }

    /// <summary>The reader writer lock source attribute.</summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    [PublicAPI]
    public sealed class ReaderWriterLockSourceAttribute : ContextPropertyAttributeBase
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
            context.Register<ReaderWriterLockHolder, ReaderWriterLockHolder>(
                                                                             new ReaderWriterLockHolder(
                                                                                                        info
                                                                                                            .GetInvokeMember
                                                                                                                <ReaderWriterLockSlim>(target))
                                                                             {
                                                                                 Name = HolderName
                                                                             });
        }

        #endregion
    }

    /// <summary>The reader writer lock behavior.</summary>
    [PublicAPI]
    public enum ReaderWriterLockBehavior
    {
        Invalid,

        /// <summary>The read.</summary>
        Read,

        /// <summary>The write.</summary>
        Write
    }

    /// <summary>The reader writer lock attribute.</summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Event | AttributeTargets.Property)]
    public sealed class ReaderWriterLockAttribute : ThreadingBaseAspect
    {
        #region Public Properties

        /// <summary>Gets or sets the lock behavior.</summary>
        /// <value>The lock behavior.</value>
        public ReaderWriterLockBehavior LockBehavior { get; set; }

        #endregion

        #region Fields

        /// <summary>The _enter.</summary>
        private Func<bool> _enter;

        /// <summary>The _exit.</summary>
        private Action _exit;

        /// <summary>The _holder.</summary>
        private ReaderWriterLockHolder _holder;

        /// <summary>The _skip.</summary>
        private Func<bool> _skip;

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
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        protected internal override void Initialize(object target, ObjectContext context, string contextName)
        {
            _holder = BaseHolder.GetOrAdd<ReaderWriterLockHolder, ReaderWriterLockHolder>(
                                                                                          context,
                                                                                          () =>
                                                                                              new ReaderWriterLockHolder(),
                                                                                          HolderName);
            switch (LockBehavior)
            {
                case ReaderWriterLockBehavior.Read:
                    _enter = () => _holder.Value.TryEnterReadLock(-1);
                    _exit  = _holder.Value.ExitReadLock;
                    _skip  = () => _holder.Value.IsReadLockHeld;
                    break;
                case ReaderWriterLockBehavior.Write:
                    _enter = () => _holder.Value.TryEnterWriteLock(-1);
                    _exit  = _holder.Value.ExitWriteLock;
                    _skip  = () => _holder.Value.IsWriteLockHeld;
                    break;
                default:
                    CommonConstants.LogCommon(false, "AOP Module: Invalid Reader WriterLogBahavior: {0}", target);
                    break;
            }

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
            if (_skip == null) return;

            var skipping = _skip();
            var taken    = false;

            try
            {
                if (!skipping) taken = _enter();

                invocation.Proceed();
            }
            finally
            {
                if (taken) _exit();
            }
        }

        #endregion
    }
}