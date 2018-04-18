#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using JetBrains.Annotations;
using Tauron.Application.Commands;

#endregion

namespace Tauron.Application
{
    /// <summary>The event target attribute.</summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    //[DebuggerNonUserCode]
    [PublicAPI]
    [MeansImplicitUse(ImplicitUseKindFlags.Access)]
    public sealed class EventTargetAttribute : MemberInfoAttribute
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EventTargetAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="EventTargetAttribute" /> Klasse.
        /// </summary>
        /// <param name="memberName">
        ///     The member name.
        /// </param>
        public EventTargetAttribute([CanBeNull] string memberName)
            : base(memberName)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EventTargetAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="EventTargetAttribute" /> Klasse.
        /// </summary>
        public EventTargetAttribute()
            : base(null)
        {
        }

        #endregion
    }

    /// <summary>The event binder.</summary>
    public static class EventBinder
    {
        private sealed class EventLinker : PipelineBase, IDisposable
        {
            //[DebuggerNonUserCode]
            internal class CommandMember
            {
                #region Constructors and Destructors

                /// <summary>
                ///     Initializes a new instance of the <see cref="CommandMember" /> class.
                ///     Initialisiert eine neue Instanz der <see cref="CommandMember" /> Klasse.
                /// </summary>
                /// <param name="name">
                ///     The name.
                /// </param>
                /// <param name="memberInfo">
                ///     The member info.
                /// </param>
                /// <param name="synchronize">
                ///     The synchronize.
                /// </param>
                public CommandMember([NotNull] string name, [NotNull] MemberInfo memberInfo, bool synchronize)
                {
                    Name        = name;
                    MemberInfo  = memberInfo;
                    Synchronize = synchronize;
                }

                #endregion

                #region Public Properties

                /// <summary>Gets the member info.</summary>
                [NotNull]
                public MemberInfo MemberInfo { get; }

                /// <summary>Gets the name.</summary>
                [NotNull]
                public string Name { get; }

                /// <summary>Gets a value indicating whether synchronize.</summary>
                public bool Synchronize { get; }

                #endregion
            }

            //[DebuggerNonUserCode]
            private class InternalEventLinker : IDisposable
            {
                #region Static Fields

                private static readonly MethodInfo Method =
                    typeof(InternalEventLinker).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                                               .First(m => m.Name == "Handler");

                #endregion

                #region Constructors and Destructors

                /// <summary>
                ///     Initializes a new instance of the <see cref="InternalEventLinker" /> class.
                ///     Initialisiert eine neue Instanz der <see cref="InternalEventLinker" /> Klasse.
                /// </summary>
                /// <param name="member">
                ///     The member.
                /// </param>
                /// <param name="event">
                ///     The event.
                /// </param>
                /// <param name="dataContext">
                ///     The data context.
                /// </param>
                /// <param name="targetName">
                ///     The target name.
                /// </param>
                /// <param name="host">
                ///     The host.
                /// </param>
                /// <param name="scheduler">
                ///     The scheduler.
                /// </param>
                public InternalEventLinker([CanBeNull] IEnumerable<CommandMember>      member, [CanBeNull] EventInfo @event, [NotNull] WeakReference dataContext, [NotNull] string targetName,
                                           [CanBeNull] WeakReference<DependencyObject> host,
                                           [NotNull]   TaskScheduler                   scheduler)
                {
                    _isDirty = (member == null) | (@event == null);

                    _scheduler   = scheduler;
                    _host        = host;
                    _event       = @event;
                    _dataContext = dataContext;
                    _targetName  = targetName;
                    FindMember(member);

                    Initialize();
                }

                #endregion

                #region Public Methods and Operators

                /// <summary>The dispose.</summary>
                public void Dispose()
                {
                    object host = _host.TypedTarget();
                    if (host == null || _delegate == null) return;

                    _event.RemoveEventHandler(host, _delegate);
                    _delegate = null;
                }

                #endregion

                #region Fields

                private readonly EventInfo _event;

                private readonly WeakReference _dataContext;

                private readonly WeakReference<DependencyObject> _host;

                private readonly TaskScheduler _scheduler;

                private readonly string _targetName;

                private Delegate _delegate;

                private ICommand _command;

                private bool _isDirty;

                private MemberInfo _member;
                private bool       _sync;

                //private bool _sync;

                #endregion

                #region Methods

                private bool EnsureCommandStade()
                {
                    if (_command != null) return true;

                    if (_member == null) return false;

                    try
                    {
                        var context = _dataContext.Target;
                        if (context == null) return false;

                        var minfo = _member as MethodInfo;
                        if (minfo != null)
                        {
                            if (minfo.ReturnType.IsAssignableFrom(typeof(ICommand))) _command = (ICommand) minfo.Invoke(context, null);
                            else _command                                                     = new MethodCommand(minfo, _dataContext);
                        }
                        else
                        {
                            var pifno = _member as PropertyInfo;
                            if (pifno != null) _command = (ICommand) pifno.GetValue(context, null);
                            else _command               = (ICommand) ((FieldInfo) _member).GetValue(context);
                        }
                    }
                    catch (InvalidCastException e)
                    {
                        CommonWpfConstans.LogCommon(true, "EventBinder: Casting Faild: {0}|{1}|{2}", _dataContext.Target,
                                                    _targetName, e);
                        _isDirty = true;
                    }
                    catch (ArgumentException e)
                    {
                        CommonWpfConstans.LogCommon(true, "EventBinder: invalid Argument: {0}|{1}|{2}", _dataContext.Target,
                                                    _targetName, e);

                        _isDirty = true;
                    }

                    return _command != null && !_isDirty;
                }

                private void FindMember([CanBeNull] IEnumerable<CommandMember> members)
                {
                    if (members == null)
                    {
                        CommonWpfConstans.LogCommon(false, "EventBinder: No Members: {0}", _dataContext.Target);
                        return;
                    }

                    var temp = members.FirstOrDefault(mem => mem.Name == _targetName);
                    if (temp == null)
                    {
                        CommonWpfConstans.LogCommon(false, "EventBinder: No Valid Member found: {0}|{1}",
                                                    _dataContext.Target, _targetName);
                        return;
                    }

                    _member = temp.MemberInfo;
                    _sync   = temp.Synchronize;
                }

                [UsedImplicitly]
                private void Handler([NotNull] object sender, [NotNull] EventArgs e)
                {
                    if (!_isDirty && !EnsureCommandStade())
                    {
                        Dispose();
                        return;
                    }

                    try
                    {
                        _scheduler.QueueTask(
                                             new UserTask(
                                                          () =>
                                                          {
                                                              var data = new EventData(sender, e);
                                                              if (_command.CanExecute(data)) _command.Execute(data);
                                                          },
                                                          _sync));
                    }
                    catch (ArgumentException)
                    {
                        _isDirty = true;
                    }
                }

                private void Initialize()
                {
                    if (_isDirty || _event == null) return;

                    object typedTarget = _host.TypedTarget();
                    if (typedTarget == null) return;

                    _delegate = Delegate.CreateDelegate(_event.EventHandlerType, this, Method);
                    _event.AddEventHandler(typedTarget, _delegate);
                }

                #endregion
            }

            #region Fields

            private readonly List<InternalEventLinker> _linkers = new List<InternalEventLinker>();

            #endregion

            #region Constructors and Destructors

            public EventLinker([NotNull] string commands, [NotNull] DependencyObject target, bool simpleMode)
                : base(target, simpleMode)
            {
                if (target == null) throw new ArgumentNullException(nameof(target));
                if (string.IsNullOrEmpty(commands)) throw new ArgumentException("Value cannot be null or empty.", nameof(commands));
                Commands = commands;
            }

            #endregion

            #region Public Properties

            /// <summary>Gets or sets the commands.</summary>
            [CanBeNull]
            public string Commands { get; set; }

            #endregion

            #region Public Methods and Operators

            /// <summary>The dispose.</summary>
            public void Dispose()
            {
                Free();
            }

            /// <summary>The bind.</summary>
            public void Bind()
            {
                DataContextChanged();
            }

            #endregion

            #region Methods

            protected override void DataContextChanged()
            {
                Free();

                if (Commands == null)
                {
                    CommonWpfConstans.LogCommon(false, "EventBinder: No Command Setted: {0}",
                                                DataContext == null ? "Unkowen" : DataContext.Target);

                    return;
                }

                var vals   = Commands.Split(':');
                var events = new Dictionary<string, string>();

                try
                {
                    for (var i = 0; i < vals.Length; i++) events[vals[i]] = vals[++i];
                }
                catch (IndexOutOfRangeException)
                {
                    CommonWpfConstans.LogCommon(false, "EventBinder: EventPairs not Valid: {0}", Commands);
                }

                if (DataContext == null) return;

                var dataContext = DataContext.Target;
                var host        = Target;
                if (host == null || dataContext == null) return;

                var hostType = host.GetType();
                var dataContextInfos =
                    (from entry in dataContext.GetType().FindMemberAttributes<EventTargetAttribute>(true)
                     select
                         new CommandMember(
                                           entry.Item2.ProvideMemberName(entry.Item1),
                                           entry.Item1,
                                           entry.Item2.Synchronize)).ToArray();

                foreach (var pair in events)
                {
                    var info = hostType.GetEvent(pair.Key);
                    if (info == null)
                    {
                        CommonWpfConstans.LogCommon(false, "EventBinder: No event Found: {0}|{1}", hostType, pair.Key);

                        return;
                    }

                    _linkers.Add(
                                 new InternalEventLinker(
                                                         dataContextInfos,
                                                         info,
                                                         DataContext,
                                                         pair.Value,
                                                         Source,
                                                         TaskScheduler ?? throw new InvalidOperationException()));
                }
            }

            private void Free()
            {
                foreach (var linker in _linkers) linker.Dispose();

                _linkers.Clear();
            }

            #endregion
        }

        #region Methods

        private static void OnEventsChanged([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d)) return;

            var simpleMode = false;

            if (!(e.NewValue is string newValue)) return;

            if (newValue.StartsWith("SimpleMode"))
            {
                simpleMode = true;
                newValue   = newValue.Remove(0, 10);
            }

            if (e.OldValue is string oldValue)
                foreach (var linker in
                    EventLinkerCollection.Where(ele => ele.Commands == oldValue && Equals(ele.Target, d)))
                {
                    linker.Commands   = newValue;
                    linker.SimpleMode = simpleMode;
                    linker.Bind();
                    return;
                }

            var temp = new EventLinker(newValue, d, simpleMode);
            EventLinkerCollection.Add(temp);
            temp.Bind();
        }

        #endregion

        #region Static Fields

        public static readonly DependencyProperty EventsProperty = DependencyProperty.RegisterAttached(
                                                                                                       "Events",
                                                                                                       typeof(string),
                                                                                                       typeof(
                                                                                                           EventBinder),
                                                                                                       new UIPropertyMetadata
                                                                                                           (null,
                                                                                                            OnEventsChanged));

        private static readonly WeakReferenceCollection<EventLinker> EventLinkerCollection =
            new WeakReferenceCollection<EventLinker>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The get events.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull]
        public static string GetEvents([NotNull] DependencyObject obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return (string) obj.GetValue(EventsProperty);
        }

        /// <summary>
        ///     The set events.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        public static void SetEvents([NotNull] DependencyObject obj, [NotNull] string value)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrEmpty(value)) throw new ArgumentException("Value cannot be null or empty.", nameof(value));
            obj.SetValue(EventsProperty, value);
        }

        #endregion
    }
}