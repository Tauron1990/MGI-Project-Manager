#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventAspectAttribute.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The event aspect attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Reflection;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.LifeTime;

#endregion

namespace Tauron.Application.Aop
{
    /// <summary>The event aspect attribute.</summary>
    [AttributeUsage(AttributeTargets.Event)]
    [PublicAPI]
    public abstract class EventAspectAttribute : AspectBaseAttribute
    {
        #region Fields

        /// <summary>The _event info.</summary>
        private EventInfo _eventInfo;

        #endregion

        #region Methods

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
            var name = invocation.Method.Name;
            var getter = name.StartsWith(AopConstants.EventAdder, StringComparison.Ordinal);
            if (_eventInfo == null)
            {
                name = name.Remove(0, getter ? AopConstants.EventAdder.Length : AopConstants.EventRemover.Length);

                _eventInfo = invocation.Method.DeclaringType.GetEvent(name, AopConstants.DefaultBindingFlags);
            }

            if (getter) OnGet(invocation, context, _eventInfo);
            else OnSet(invocation, context, _eventInfo);
        }

        /// <summary>
        ///     The on get.
        /// </summary>
        /// <param name="invocation">
        ///     The invocation.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="eventInfo">
        ///     The event info.
        /// </param>
        protected virtual void OnGet([NotNull] IInvocation invocation, [NotNull] ObjectContext context, [NotNull] EventInfo eventInfo)
        {
            if (invocation == null) throw new ArgumentNullException(nameof(invocation));
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (eventInfo == null) throw new ArgumentNullException(nameof(eventInfo));
            invocation.Proceed();
        }

        /// <summary>
        ///     The on set.
        /// </summary>
        /// <param name="invocation">
        ///     The invocation.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="eventInfo">
        ///     The event info.
        /// </param>
        protected virtual void OnSet([NotNull] IInvocation invocation, [NotNull] ObjectContext context, [NotNull] EventInfo eventInfo)
        {
            if (invocation == null) throw new ArgumentNullException(nameof(invocation));
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (eventInfo == null) throw new ArgumentNullException(nameof(eventInfo));

            invocation.Proceed();
        }

        #endregion
    }
}