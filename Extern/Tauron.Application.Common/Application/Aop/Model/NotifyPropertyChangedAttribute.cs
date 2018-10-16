#region

using System;
using System.ComponentModel;
using System.Linq;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using Tauron.Application.Ioc;
using Tauron.Application.Ioc.LifeTime;

#endregion

namespace Tauron.Application.Aop.Model
{
    /// <summary>The notify property changed attribute.</summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class NotifyPropertyChangedAttribute : AspectBaseAttribute
    {
        #region Fields

        private Action<string> _eventInvoker;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotifyPropertyChangedAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="NotifyPropertyChangedAttribute" /> Klasse.
        ///     Initializes a new instance of the <see cref="NotifyPropertyChangedAttribute" /> class.
        /// </summary>
        public NotifyPropertyChangedAttribute()
        {
            AlternativePropertyChangedName = "OnPropertyChanged";
            Order = 900;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the alternative property changed name.</summary>
        /// <value>The alternative property changed name.</value>
        [NotNull]
        public string AlternativePropertyChangedName { get; set; }

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
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrEmpty(contextName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(contextName));
            var metod = target as INotifyPropertyChangedMethod;
            if (metod != null)
            {
                _eventInvoker = metod.OnPropertyChanged;
            }
            else
            {
                var info =
                    target.GetType()
                        .GetMethods(AopConstants.DefaultBindingFlags)
                        .FirstOrDefault(
                            metodInfo =>
                                metodInfo.Name == AlternativePropertyChangedName
                                && metodInfo.ReturnType == typeof(void));
                if (info != null && info.GetParameters().Length == 1)
                {
                    var parameterType = info.GetParameters()[0].ParameterType;
                    if (parameterType == typeof(PropertyChangedEventArgs))
                        _eventInvoker = s => info.Invoke(target, new PropertyChangedEventArgs(s));
                    else if (parameterType == typeof(string)) _eventInvoker = s => info.Invoke(target, s);
                    else
                        CommonConstants.LogCommon(false,
                            "AOP Module: No PropertyChanged Method Found: Class:{0} AltName:{1}",
                            target,
                            AlternativePropertyChangedName);
                }
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
            if (invocation == null) throw new ArgumentNullException(nameof(invocation));
            if (context == null) throw new ArgumentNullException(nameof(context));
            invocation.Proceed();

            if (_eventInvoker == null) return;

            if (invocation.Method.Name.StartsWith(AopConstants.PropertySetter, StringComparison.Ordinal)
                && invocation.Method.IsSpecialName)
                _eventInvoker(invocation.Method.Name.Remove(0, AopConstants.PropertySetter.Length));
        }

        #endregion
    }
}