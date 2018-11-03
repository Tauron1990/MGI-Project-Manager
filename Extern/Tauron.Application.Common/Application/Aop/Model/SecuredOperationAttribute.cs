#region

using System;
using System.Security;
using System.Threading;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Tauron.Application.Ioc.LifeTime;

#endregion

namespace Tauron.Application.Aop.Model
{
    /// <summary>The secured operation attribute.</summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, AllowMultiple = true)]
    [PublicAPI]
    public sealed class SecuredOperationAttribute : AspectBaseAttribute
    {
        #region Constructors and Destructors

        public SecuredOperationAttribute([NotNull] string roles)
        {
            if (roles == null) throw new ArgumentNullException(nameof(roles));

            Roles = roles;
        }

        #endregion

        #region Public Properties

        [NotNull] public string Roles { get; }

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
        /// <exception cref="SecurityException">
        /// </exception>
        protected override void Intercept(IInvocation invocation, ObjectContext context)
        {
            var able = invocation.InvocationTarget as ISecurable;

            if (!able?.IsUserInRole(Thread.CurrentPrincipal.Identity, Roles) == true)
                throw new SecurityException(
                    $"The user {Thread.CurrentPrincipal.Identity.Name} does not have the required permissions.");

            invocation.Proceed();
        }

        #endregion

        #region Fields

        #endregion
    }
}