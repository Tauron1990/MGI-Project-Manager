#region

using System;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using NLog;
using Tauron.Application.Ioc.LifeTime;

#endregion

namespace Tauron.Application.Aop.Model
{
    /// <summary>The trace aspect options.</summary>
    [Flags]
    [PublicAPI]
    public enum TraceAspectOptions
    {
        /// <summary>The none.</summary>
        None = 0,

        /// <summary>The parameter type.</summary>
        ParameterType = 1,

        /// <summary>The parameter name.</summary>
        ParameterName = 2,

        /// <summary>The parameter value.</summary>
        ParameterValue = 4,

        /// <summary>The return value.</summary>
        ReturnValue = 8
    }

    /// <summary>The trace attributte.</summary>
    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class TraceAttribute : AspectBaseAttribute
    {
        /// <summary>The logger helper.</summary>
        private class LoggerHelper
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="LoggerHelper" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="LoggerHelper" /> Klasse.
            ///     Initializes a new instance of the <see cref="LoggerHelper" /> class.
            /// </summary>
            /// <param name="value">
            ///     The value.
            /// </param>
            /// <param name="type">
            ///     The type.
            /// </param>
            public LoggerHelper(bool value, bool type)
            {
                _value = value;
                _type = type;
            }

            #endregion

            #region Properties

            /// <summary>The _parm names.</summary>
            [NotNull]
            private string[] ParmNames { get; set; }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     The log.
            /// </summary>
            /// <param name="entry">
            ///     The entry.
            /// </param>
            /// <param name="invocation">
            ///     The invocation.
            /// </param>
            public void Log([NotNull] LogEventInfo entry, [NotNull] IInvocation invocation)
            {
                if (entry == null) throw new ArgumentNullException(nameof(entry));
                if (invocation == null) throw new ArgumentNullException(nameof(invocation));
                lock (this)
                {
                    if (!_initialized) Initialize(invocation.Method);
                }
                
                entry.Properties["Parameters"] = _stringNmes;
                entry.Properties["ParameterTypes"] = _types;

                if (!_value) return;

                var args = invocation.Arguments;
                for (var i = 0; i < ParmNames.Length; i++) entry.Properties["Parameter:" + ParmNames[i]] = args[i];
            }

            #endregion

            #region Methods

            /// <summary>
            ///     The initialize.
            /// </summary>
            /// <param name="info">
            ///     The info.
            /// </param>
            private void Initialize([NotNull] MethodInfo info)
            {
                if (info == null) throw new ArgumentNullException(nameof(info));
                var parms = info.GetParameters();
                ParmNames = parms.Select(parm => parm.Name).ToArray();
                _stringNmes = ParmNames.Aggregate((working, next) => working + ", " + next);
                if (_type)
                    _types = parms.Select(parm => parm.ParameterType)
                        .Aggregate("Types: ", (s, type1) => s + type1.ToString() + ", ");

                _initialized = true;
            }

            #endregion

            #region Fields

            /// <summary>The _type.</summary>
            private readonly bool _type;

            /// <summary>The _value.</summary>
            private readonly bool _value;

            /// <summary>The _initialized.</summary>
            private bool _initialized;

            /// <summary>The _string nmes.</summary>
            private string _stringNmes;

            /// <summary>The _types.</summary>
            private string _types;

            #endregion
        }

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TraceAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="TraceAttribute" /> Klasse.
        ///     Initializes a new instance of the <see cref="TraceAttribute" /> class.
        /// </summary>
        public TraceAttribute()
        {
            Order = 100;
            TraceEventType = LogLevel.Info;
            LogOptions = TraceAspectOptions.ParameterName;
            LogTitle = string.Empty;
        }

        #endregion

        #region Fields

        /// <summary>The _helper.</summary>
        private LoggerHelper _helper;

        /// <summary>The _log return.</summary>
        private bool _logReturn;


        #endregion

        #region Public Properties

        /// <summary>Gets or sets the log options.</summary>
        /// <value>The log options.</value>
        public TraceAspectOptions LogOptions { get; set; }

        /// <summary>Gets or sets the log title.</summary>
        /// <value>The log title.</value>
        [CanBeNull]
        public string LogTitle { get; set; }

        /// <summary>Gets or sets the trace event type.</summary>
        /// <value>The trace event type.</value>
        public LogLevel TraceEventType { get; set; }


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
            base.Initialize(target, context, contextName);

            var logParameterName = LogOptions.HasFlag(TraceAspectOptions.ParameterName);
            var logParameterType = LogOptions.HasFlag(TraceAspectOptions.ParameterType);
            var logparameterValue = LogOptions.HasFlag(TraceAspectOptions.ParameterValue);

            _logReturn = LogOptions.HasFlag(TraceAspectOptions.ReturnValue);

            if (logParameterName || logParameterType || logparameterValue) _helper = new LoggerHelper(logparameterValue, logParameterType);
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
        /// <exception cref="Exception">
        /// </exception>
        protected override void Intercept(IInvocation invocation, ObjectContext context)
        {
            var logger = LogManager.GetLogger(LogTitle, invocation.TargetType);
            
            var isLoggingEnabled = LogManager.IsLoggingEnabled();
            if (isLoggingEnabled)
            {
                var entry = LogEventInfo.Create(TraceEventType, LogTitle, $"Enter Method: {invocation.Method.Name}");
                
                _helper?.Log(entry, invocation);

                logger.Log(entry);
            }

            try
            {
                invocation.Proceed();
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }

            if (!isLoggingEnabled) return;

            var entry2 = LogEventInfo.Create(TraceEventType, LogTitle, $"Exit Method: {invocation.Method.Name}"); 
            
            entry2.Properties["ReturnValue"] = invocation.ReturnValue;

           logger.Log(entry2);
        }

        #endregion
    }
}