#region

using System;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Tauron.Application.Ioc.LifeTime;

#endregion

namespace Tauron.Application.Aop
{
    /// <summary>The method aspect attribute.</summary>
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class MethodAspectAttribute : AspectBaseAttribute
    {
        #region Methods

        /// <summary>
        ///     The enter method.
        /// </summary>
        /// <param name="invocation">
        ///     The invocation.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        protected virtual void EnterMethod([NotNull] IInvocation invocation, [NotNull] ObjectContext context)
        {
            if (invocation == null) throw new ArgumentNullException(nameof(invocation));
            if (context == null) throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        ///     The execute method.
        /// </summary>
        /// <param name="invocation">
        ///     The invocation.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        protected virtual void ExecuteMethod([NotNull] IInvocation invocation, [NotNull] ObjectContext context)
        {
            if (invocation == null) throw new ArgumentNullException(nameof(invocation));
            if (context == null) throw new ArgumentNullException(nameof(context));

            invocation.Proceed();
        }

        /// <summary>
        ///     The exit method.
        /// </summary>
        /// <param name="invocation">
        ///     The invocation.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        protected virtual void ExitMethod([NotNull] IInvocation invocation, [NotNull] ObjectContext context)
        {
            if (invocation == null) throw new ArgumentNullException(nameof(invocation));
            if (context == null) throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        ///     The finally method.
        /// </summary>
        /// <param name="invocation">
        ///     The invocation.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        protected virtual void FinallyMethod([NotNull] IInvocation invocation, [NotNull] ObjectContext context)
        {
            if (invocation == null) throw new ArgumentNullException(nameof(invocation));
            if (context == null) throw new ArgumentNullException(nameof(context));
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
            try
            {
                EnterMethod(invocation, context);
                ExecuteMethod(invocation, context);
                ExitMethod(invocation, context);
            }
            catch (Exception e)
            {
                if (MethodException(invocation, e, context))
                    throw;
            }
            finally
            {
                FinallyMethod(invocation, context);
            }
        }

        /// <summary>
        ///     The method exception.
        /// </summary>
        /// <param name="invocation">
        ///     The invocation.
        /// </param>
        /// <param name="exception">
        ///     The e.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        protected virtual bool MethodException([NotNull] IInvocation   invocation, [NotNull] Exception exception,
                                               [NotNull] ObjectContext context)
        {
            if (invocation == null) throw new ArgumentNullException(nameof(invocation));
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            if (context == null) throw new ArgumentNullException(nameof(context));

            return true;
        }

        #endregion
    }
}