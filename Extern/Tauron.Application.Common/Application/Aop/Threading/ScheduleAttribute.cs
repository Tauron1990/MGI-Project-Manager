#region

using System;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Tauron.Application.Ioc.LifeTime;

#endregion

namespace Tauron.Application.Aop.Threading
{
    /// <summary>The task option.</summary>
    public enum TaskOption
    {
        /// <summary>The worker.</summary>
        Worker,

        /// <summary>The task.</summary>
        Task,

        /// <summary>The ui thread.</summary>
        UIThread
    }

    /// <summary>The schedule attribute.</summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ScheduleAttribute : AspectBaseAttribute
    {
        #region Fields

        /// <summary>The _is ok.</summary>
        private bool _isOk;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ScheduleAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ScheduleAttribute" /> Klasse.
        ///     Initializes a new instance of the <see cref="ScheduleAttribute" /> class.
        /// </summary>
        public ScheduleAttribute()
        {
            Order = 0;
            CreationOptions = TaskCreationOptions.None;
            TaskOption = TaskOption.Worker;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The create.
        /// </summary>
        /// <param name="info">
        ///     The info.
        /// </param>
        /// <returns>
        ///     The <see cref="IInterceptor" />.
        /// </returns>
        [NotNull]
        public override IInterceptor Create([NotNull] MemberInfo info)
        {
            _isOk = ((MethodInfo) info).ReturnType == typeof(void);

            CommonConstants.LogCommon(false,
                "AOP Module: The member {0}.{1} has no Void Return",
                info.DeclaringType.FullName,
                info.Name);

            return base.Create(info);
        }

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
            if (!_isOk)
            {
                invocation.Proceed();
                return;
            }

            switch (TaskOption)
            {
                case TaskOption.Worker:
                    CommonApplication.QueueWorkitemAsync(invocation.Proceed, false);
                    break;
                case TaskOption.Task:
                    Task.Factory.StartNew(invocation.Proceed, CreationOptions);
                    break;
                case TaskOption.UIThread:
                    CommonApplication.QueueWorkitemAsync(invocation.Proceed, true);
                    break;
                default:
                    CommonConstants.LogCommon(false, "Invalid Schedule TaskOption: {0}.{1}", invocation.TargetType,
                        invocation.Method);
                    invocation.Proceed();
                    break;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the creation options.</summary>
        /// <value>The creation options.</value>
        public TaskCreationOptions CreationOptions { get; set; }

        /// <summary>Gets or sets the task option.</summary>
        /// <value>The task option.</value>
        public TaskOption TaskOption { get; set; }

        #endregion
    }
}