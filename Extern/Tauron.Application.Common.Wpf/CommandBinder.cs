#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>The command binder.</summary>
    [PublicAPI]
    public static class CommandBinder
    {
        #region Public Properties

        /// <summary>Gets or sets a value indicating whether auto register.</summary>
        public static bool AutoRegister { get; set; }

        #endregion

        // [DebuggerNonUserCode]
        private class CommandLinker : PipelineBase
        {
            #region Constructors and Destructors

            public CommandLinker([NotNull] DependencyObject element)
                : base(element, false)
            {
            }

            #endregion

            #region Public Properties

            /// <summary>Gets or sets the command target.</summary>
            [CanBeNull]
            public string CommandTarget { get; set; }

            #endregion

            //[DebuggerNonUserCode]
            private class CommandFactory
            {
                #region Fields

                private bool _isSync;

                #endregion

                #region Constructors and Destructors

                /// <summary>
                ///     Initializes a new instance of the <see cref="CommandFactory" /> class.
                ///     Initialisiert eine neue Instanz der <see cref="CommandFactory" /> Klasse.
                /// </summary>
                /// <param name="target">
                ///     The target.
                /// </param>
                /// <param name="name">
                ///     The name.
                /// </param>
                public CommandFactory([NotNull] WeakReference target, [NotNull] string name)
                {
                    Target = target ?? throw new ArgumentNullException(nameof(target));
                    Name = name ?? throw new ArgumentNullException(nameof(name));
                }

                #endregion

                //[DebuggerNonUserCode]
                private class ParameterMapper
                {
                    #region Constructors and Destructors

                    public ParameterMapper([NotNull] MethodInfo method, [NotNull] object firstArg)
                    {
                        if (method == null) throw new ArgumentNullException(nameof(method));
                        _method = method;
                        _firstArg = firstArg ?? throw new ArgumentNullException(nameof(firstArg));
                        _isParameter = method.GetParameters().Length == 1;
                        _isAsync = method.ReturnType.IsAssignableFrom(typeof(Task));
                    }

                    #endregion

                    #region Fields

                    private readonly object _firstArg;

                    private readonly bool _isParameter;

                    private readonly MethodInfo _method;

                    private readonly bool _isAsync;

#pragma warning disable 169
                    //private readonly Func<object, object[], RoutedEventArgs> _parmeterMapper;
#pragma warning restore 169

                    #endregion

                    #region Public Methods and Operators

                    /// <summary>
                    ///     The can execute.
                    /// </summary>
                    /// <param name="sender">
                    ///     The sender.
                    /// </param>
                    /// <param name="e">
                    ///     The e.
                    /// </param>
                    public void CanExecute([NotNull] object sender, [NotNull] CanExecuteRoutedEventArgs e)
                    {
                        if (sender == null) throw new ArgumentNullException(nameof(sender));
                        if (e == null) throw new ArgumentNullException(nameof(e));
                        var args = _isParameter ? new[] {e.Parameter} : new object[0];

                        e.CanExecute = _isAsync
                            ? _method.Invoke<Task<bool>>(_firstArg, args).Result
                            : _method.Invoke<bool>(_firstArg, args);
                    }

                    /// <summary>
                    ///     The execute.
                    /// </summary>
                    /// <param name="sender">
                    ///     The sender.
                    /// </param>
                    /// <param name="e">
                    ///     The e.
                    /// </param>
                    public void Execute([NotNull] object sender, [NotNull] ExecutedRoutedEventArgs e)
                    {
                        if (sender == null) throw new ArgumentNullException(nameof(sender));
                        if (e == null) throw new ArgumentNullException(nameof(e));

                        if (_isParameter)
                            _method.Invoke(_firstArg, e.Parameter);
                        else
                            _method.Invoke(_firstArg, null);
                    }

                    #endregion
                }

                //[DebuggerNonUserCode]
                private class TaskFactory
                {
                    #region Constructors and Destructors

                    /// <summary>
                    ///     Initializes a new instance of the <see cref="TaskFactory" /> class.
                    ///     Initialisiert eine neue Instanz der <see cref="TaskFactory" /> Klasse.
                    /// </summary>
                    /// <param name="del">
                    ///     The del.
                    /// </param>
                    /// <param name="scheduler">
                    ///     The scheduler.
                    /// </param>
                    /// <param name="sync">
                    ///     The sync.
                    /// </param>
                    public TaskFactory([NotNull] Delegate del, [NotNull] ITaskScheduler scheduler, bool sync)
                    {
                        _del = del ?? throw new ArgumentNullException(nameof(del));
                        _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
                        _sync = sync;
                    }

                    #endregion

                    #region Public Methods and Operators

                    /// <summary>
                    ///     The handler.
                    /// </summary>
                    /// <param name="parm1">
                    ///     The parm 1.
                    /// </param>
                    /// <param name="parm2">
                    ///     The parm 2.
                    /// </param>
                    public void Handler([NotNull] object parm1, [NotNull] object parm2)
                    {
                        _scheduler.QueueTask(new UserTask(() => _del.DynamicInvoke(parm1, parm2), _sync));
                    }

                    #endregion

                    #region Fields

                    private readonly Delegate _del;

                    private readonly ITaskScheduler _scheduler;

                    private readonly bool _sync;

                    #endregion
                }

                private class MemberInfoHelper
                {
                    public static readonly MethodInfo CanExecuteMethod =
                        typeof(MemberInfoHelper).GetMethod("CanExecute",
                            new[] {typeof(object), typeof(CanExecuteRoutedEventArgs)});

                    private readonly MemberInfo _info;
                    private readonly object _target;

                    public MemberInfoHelper([NotNull] MemberInfo info, [NotNull] object target)
                    {
                        _info = info;
                        _target = target;
                    }

                    [UsedImplicitly]
                    public void CanExecute([NotNull] object sender, [NotNull] CanExecuteRoutedEventArgs e)
                    {
                        e.CanExecute = _info.GetInvokeMember<bool>(_target);
                    }
                }

                #region Public Properties

                /// <summary>Gets the last command.</summary>
                [CanBeNull]
                public ICommand LastCommand { get; private set; }

                /// <summary>Gets or sets the name.</summary>
                [NotNull]
                public string Name { private get; set; }

                /// <summary>Gets or sets the target.</summary>
                [CanBeNull]
                public WeakReference Target { private get; set; }

                #endregion

                #region Public Methods and Operators

                /// <summary>
                ///     The free.
                /// </summary>
                /// <param name="command">
                ///     The command.
                /// </param>
                /// <param name="targetObject">
                ///     The target object.
                /// </param>
                public static void Free([CanBeNull] ICommand command, [NotNull] DependencyObject targetObject)
                {
                    if (targetObject == null) throw new ArgumentNullException(nameof(targetObject));
                    var window = Window.GetWindow(targetObject);
                    if (window == null || command == null) return;

                    for (var i = 0; i < window.CommandBindings.Count; i++)
                    {
                        if (window.CommandBindings[i].Command != command) continue;

                        window.CommandBindings.RemoveAt(i);
                        return;
                    }
                }

                /// <summary>
                ///     The connect.
                /// </summary>
                /// <param name="command">
                ///     The command.
                /// </param>
                /// <param name="targetObject">
                ///     The target object.
                /// </param>
                /// <param name="scheduler">
                ///     The scheduler.
                /// </param>
                /// <param name="commandName"></param>
                public void Connect([NotNull] ICommand command, [NotNull] DependencyObject targetObject,
                    [NotNull] ITaskScheduler scheduler, [NotNull] string commandName)
                {
                    if (scheduler == null) throw new ArgumentNullException(nameof(scheduler));
                    var target = Target?.Target;
                    if (target == null) return;

                    var targetType = target.GetType();

                    LastCommand = command;

                    var pair = FindCommandPair(targetType, out var ok);

                    var temp = command as RoutedCommand;
                    var binding = SetCommandBinding(targetObject, temp);

                    if (binding == null || !ok || pair == null) return;


                    ExecutedRoutedEventHandler del = null;
                    if (pair.Item1 != null)
                        del =
                            Delegate.CreateDelegate(typeof(ExecutedRoutedEventHandler), target, pair.Item1, false)
                                .As<ExecutedRoutedEventHandler>() ?? new ParameterMapper(pair.Item1, target).Execute;

                    CanExecuteRoutedEventHandler del2 = null;
                    if (pair.Item2 != null)
                    {
                        var method = pair.Item2 as MethodInfo;
                        var localTarget = target;

                        if (method == null)
                        {
                            method = MemberInfoHelper.CanExecuteMethod;
                            localTarget = new MemberInfoHelper(pair.Item2, target);
                        }

                        del2 =
                            Delegate.CreateDelegate(typeof(CanExecuteRoutedEventHandler), localTarget, method, false)
                                .As<CanExecuteRoutedEventHandler>()
                            ?? new ParameterMapper(method, localTarget).CanExecute;
                    }

                    if (del != null) binding.Executed += new TaskFactory(del, scheduler, _isSync).Handler;
                    else
                        CommonWpfConstans.LogCommon(false, "CommandBinder: No Compatible method Found: {0}",
                            commandName);

                    if (del2 != null) binding.CanExecute += del2;
                }

                /// <summary>The get command.</summary>
                /// <returns>
                ///     The <see cref="ICommand" />.
                /// </returns>
                [CanBeNull]
                public ICommand GetCommand()
                {
                    return LastCommand;

                    //object target = Target?.Target;
                    //if (target == null) return null;

                    //Type targetType = target.GetType();

                    //return FindCommandToDelegate(targetType, target);
                }

                #endregion

                #region Methods

                [CanBeNull]
                private static CommandBinding SetCommandBinding([CanBeNull] DependencyObject obj,
                    [CanBeNull] ICommand command)
                {
                    while (obj != null && !(obj is Window || obj is UserControl || GetCommandScope(obj)))
                    {
                        var temp = new FrameworkObject(obj, false);
                        obj = temp.Parent ?? temp.VisualParent;
                    }

                    if (obj == null || command == null) return null;

                    var commandBindings = ((UIElement) obj).CommandBindings;

                    var binding =
                        commandBindings.OfType<CommandBinding>().FirstOrDefault(cb => cb.Command == command);
                    if (binding != null) return null;

                    binding = new CommandBinding(command);
                    commandBindings.Add(binding);
                    return binding;
                }

                [CanBeNull]
                private Tuple<MethodInfo, MemberInfo> FindCommandPair([NotNull] IReflect targetType, out bool finded)
                {
                    finded = false;
                    var methods =
                        targetType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    var main = (from method in methods
                        let attr = method.GetCustomAttribute<CommandTargetAttribute>()
                        where attr != null && attr.ProvideMemberName(method) == Name
                        select new {Method = method, IsSync = attr.Synchronize}).FirstOrDefault();
                    if (main == null)
                    {
                        CommonWpfConstans.LogCommon(false, "CommandBinder: No Command-Method Found: {0}", Name);
                        return null;
                    }

                    finded = true;

                    _isSync = main.IsSync;

                    var mainAttr = main.Method.GetCustomAttribute<CommandTargetAttribute>();
                    MemberInfo second = null;
                    foreach (var m in targetType.GetMembers(BindingFlags.Instance | BindingFlags.Public |
                                                            BindingFlags.NonPublic))
                    {
                        var attr = m.GetCustomAttribute<CommandTargetAttribute>();
                        if (attr == null) continue;

                        var name = attr.ProvideMemberName(m);
                        if (mainAttr?.CanExecuteMember != null)
                        {
                            if (mainAttr.CanExecuteMember != name) continue;
                        }
                        else
                        {
                            if (Name != name && m == main.Method) continue;

                            if ("Can" + Name != name) continue;
                        }

                        second = m;
                        break;
                    }

                    return Tuple.Create(main.Method, second);
                }

                //[CanBeNull]
                //private ICommand FindCommandToDelegate([NotNull] IReflect targetType, [NotNull] object target)
                //{
                //    var mem =
                //    (from member in
                //        targetType.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                //        let attr = member.GetCustomAttribute<CommandTargetAttribute>()
                //        where attr != null && attr.ProvideMemberName(member) == Name
                //        select member).FirstOrDefault();

                //    if (mem != null) return mem.GetInvokeMember<ICommand>(target, null);

                //    CommonWpfConstans.LogCommon(false, "CommandBinde: No Possible Command Found: {0}", Name);

                //    return null;
                //}

                #endregion
            }

            //[DebuggerNonUserCode]
            private class PropertySearcher
            {
                #region Constructors and Destructors

                /// <summary>
                ///     Initializes a new instance of the <see cref="PropertySearcher" /> class.
                ///     Initialisiert eine neue Instanz der <see cref="PropertySearcher" /> Klasse.
                /// </summary>
                /// <param name="target">
                ///     The target.
                /// </param>
                /// <param name="customName">
                ///     The custom name.
                /// </param>
                /// <param name="command">
                ///     The command.
                /// </param>
                public PropertySearcher([NotNull] WeakReference<DependencyObject> target, [NotNull] string customName,
                    [NotNull] ICommand command)
                {
                    Target = target ?? throw new ArgumentNullException(nameof(target));
                    CustomName = customName ?? throw new ArgumentNullException(nameof(customName));
                    Command = command ?? throw new ArgumentNullException(nameof(command));

                    _changedFlags = PropertyFlags.All;
                }

                #endregion

                #region Properties

                [NotNull] private WeakReference<DependencyObject> Target { get; }

                #endregion

                #region Public Methods and Operators

                /// <summary>The set command.</summary>
                public void SetCommand()
                {
                    try
                    {
                        var commandChanged = _changedFlags.HasFlag(PropertyFlags.Command);
                        var customNameChanged = _changedFlags.HasFlag(PropertyFlags.CustomName);

                        if (customNameChanged)
                        {
                            var dependencyObject = Target.TypedTarget();
                            if (dependencyObject != null)
                            {
                                var tarType = dependencyObject.GetType();
                                _prop = tarType.GetProperty(CustomName);
                                if (_prop != null
                                    && (!_prop.CanWrite || !typeof(ICommand).IsAssignableFrom(_prop.PropertyType)))
                                {
                                    var typeName = tarType.ToString();
                                    var propName = _prop == null ? CustomName + "(Not Found)" : _prop.Name;

                                    CommonWpfConstans.LogCommon(false,
                                        "CommandBinder: FoundetProperty Incompatible: {0}:{1}", typeName, propName);
                                    _prop = null;
                                }
                                else
                                {
                                    commandChanged = true;
                                }
                            }
                        }

                        if (commandChanged && _prop != null) _prop.SetValue(Target.TypedTarget(), Command, null);
                    }
                    finally
                    {
                        _changedFlags = PropertyFlags.None;
                    }
                }

                #endregion

                #region Enums

                [Flags]
                private enum PropertyFlags
                {
                    /// <summary>The none.</summary>
                    None = 0,

                    /// <summary>The custom name.</summary>
                    CustomName = 1,

                    /// <summary>The command.</summary>
                    Command = 2,

                    /// <summary>The all.</summary>
                    All = 3
                }

                #endregion

                #region Fields

                private PropertyFlags _changedFlags;

                private WeakReference<ICommand> _command;

                private string _customName;

                private PropertyInfo _prop;

                #endregion

                #region Public Properties

                /// <summary>Gets or sets the command.</summary>
                [CanBeNull]
                public ICommand Command
                {
                    private get { return _command.TypedTarget(); }

                    set
                    {
                        if (_command != null && _command.TypedTarget() == value) return;

                        _command = new WeakReference<ICommand>(value);
                        _changedFlags |= PropertyFlags.Command;
                    }
                }

                /// <summary>Gets or sets the custom name.</summary>
                [NotNull]
                public string CustomName
                {
                    private get { return _customName; }

                    set
                    {
                        if (_customName == value) return;

                        _customName = value;
                        _changedFlags |= PropertyFlags.CustomName;
                    }
                }

                #endregion
            }

            #region Fields

            private CommandFactory _factory;

            private PropertySearcher _searcher;

            #endregion

            #region Public Methods and Operators

            /// <summary>The bind.</summary>
            public void Bind()
            {
                Free();

                if (DataContext == null) return;

                var dataContext = DataContext.Target;
                var target = Target;
                var commandTarget = CommandTarget;
                if (dataContext == null || target == null || commandTarget == null)
                {
                    CommonWpfConstans.LogCommon(false, "CommandBinder: No Binding: {0}", commandTarget ?? string.Empty);

                    return;
                }

                var customProperty = GetCustomPropertyName(target);
                var useDirect = GetUseDirect(target);
                var targetCommand = GetTargetCommand(target);
                if (targetCommand == null)
                {
                    CommonWpfConstans.LogCommon(false, "CommandBinder: No ICommand: {0}", commandTarget);
                    return;
                }

                if (_factory == null)
                {
                    _factory = new CommandFactory(DataContext, commandTarget);
                }
                else
                {
                    _factory.Name = commandTarget;
                    _factory.Target = DataContext;
                }

                if (!useDirect) _factory.Connect(targetCommand, target, TaskScheduler, commandTarget);

                if (_searcher == null)
                {
                    _searcher = new PropertySearcher(Source ?? throw new InvalidOperationException(), customProperty,
                        targetCommand);
                }
                else
                {
                    _searcher.CustomName = customProperty;
                    _searcher.Command = _factory.GetCommand(); //TODO GetCommand Not Correct
                }

                _searcher.SetCommand();
            }

            /// <summary>
            ///     The reset command.
            /// </summary>
            /// <param name="command">
            ///     The command.
            /// </param>
            public void ResetCommand([NotNull] ICommand command)
            {
                var target = Target;
                if (target == null) return;

                SetTargetCommand(target, command);
            }

            #endregion

            #region Methods

            /// <summary>The data context changed.</summary>
            protected override void DataContextChanged()
            {
                Bind();
            }

            private void Free()
            {
                if (_factory != null && Target != null) CommandFactory.Free(_factory.LastCommand, Target);
            }

            #endregion
        }

        #region Static Fields

        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command",
            typeof(string),
            typeof(
                CommandBinder
            ),
            new UIPropertyMetadata
            (null,
                OnCommandChanged));

        public static readonly DependencyProperty CommandScopeProperty =
            DependencyProperty.RegisterAttached(
                "CommandScope",
                typeof(bool),
                typeof(CommandBinder),
                new PropertyMetadata(false));

        public static readonly DependencyProperty CustomPropertyNameProperty =
            DependencyProperty.RegisterAttached(
                "CustomPropertyName",
                typeof(string),
                typeof(CommandBinder),
                new UIPropertyMetadata("Command", OnCommandStadeChanged));

        public static readonly DependencyProperty TargetCommandProperty =
            DependencyProperty.RegisterAttached(
                "TargetCommand",
                typeof(ICommand),
                typeof(CommandBinder),
                new UIPropertyMetadata(null, OnCommandStadeChanged));

        public static readonly DependencyProperty UseDirectProperty = DependencyProperty.RegisterAttached(
            "UseDirect",
            typeof(bool),
            typeof(
                CommandBinder
            ),
            new UIPropertyMetadata
            (false,
                OnCommandStadeChanged));

        private static readonly List<RoutedCommand> Commands = new List<RoutedCommand>();

        private static readonly WeakReferenceCollection<CommandLinker> LinkerCollection =
            new WeakReferenceCollection<CommandLinker>();

        private static bool _isIn;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The find.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <returns>
        ///     The <see cref="RoutedCommand" />.
        /// </returns>
        [CanBeNull]
        public static RoutedCommand Find([NotNull] string name)
        {
            var val = Commands.Find(com => com.Name == name);
            if (val == null && AutoRegister) val = Register(name, name);

            return val;
        }

        /// <summary>
        ///     The get command.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull]
        public static string GetCommand([NotNull] DependencyObject obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return (string) obj.GetValue(CommandProperty);
        }

        /// <summary>
        ///     The get command scope.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool GetCommandScope([NotNull] DependencyObject obj)
        {
            return (bool) obj.GetValue(CommandScopeProperty);
        }

        /// <summary>
        ///     The get custom property name.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull]
        public static string GetCustomPropertyName([NotNull] DependencyObject obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return (string) obj.GetValue(CustomPropertyNameProperty);
        }

        /// <summary>
        ///     The get target command.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <returns>
        ///     The <see cref="ICommand" />.
        /// </returns>
        [CanBeNull]
        public static ICommand GetTargetCommand([NotNull] DependencyObject obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return (ICommand) obj.GetValue(TargetCommandProperty);
        }

        /// <summary>
        ///     The get use direct.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool GetUseDirect([NotNull] DependencyObject obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return (bool) obj.GetValue(UseDirectProperty);
        }

        /// <summary>
        ///     The register.
        /// </summary>
        /// <param name="command">
        ///     The command.
        /// </param>
        public static void Register([NotNull] RoutedCommand command)
        {
            if (Commands.Any(com => com.Name == command.Name)) return;

            Commands.Add(command);

            foreach (var linker in LinkerCollection)
            {
                if (linker.CommandTarget != command.Name) return;

                linker.ResetCommand(command);
                linker.Bind();
            }
        }

        /// <summary>
        ///     The register.
        /// </summary>
        /// <param name="text">
        ///     The text.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <returns>
        ///     The <see cref="RoutedUICommand" />.
        /// </returns>
        [NotNull]
        public static RoutedUICommand Register([NotNull] string text, [NotNull] string name)
        {
            var command = new RoutedUICommand(text, name, typeof(CommandBinder));
            Register(command);
            return command;
        }

        /// <summary>
        ///     The set command.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        public static void SetCommand([NotNull] DependencyObject obj, [NotNull] string value)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            obj.SetValue(CommandProperty, value);
        }

        /// <summary>
        ///     The set command scope.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        public static void SetCommandScope([NotNull] DependencyObject obj, bool value)
        {
            obj.SetValue(CommandScopeProperty, value);
        }

        /// <summary>
        ///     The set custom property name.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        public static void SetCustomPropertyName([NotNull] DependencyObject obj, [NotNull] string value)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            obj.SetValue(CustomPropertyNameProperty, value);
        }

        /// <summary>
        ///     The set target command.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        public static void SetTargetCommand([NotNull] DependencyObject obj, [NotNull] ICommand value)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            obj.SetValue(TargetCommandProperty, value);
        }

        /// <summary>
        ///     The set use direct.
        /// </summary>
        /// <param name="obj">
        ///     The obj.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        public static void SetUseDirect([NotNull] DependencyObject obj, bool value)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            obj.SetValue(UseDirectProperty, value);
        }

        #endregion

        #region Methods

        private static void OnCommandChanged([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d)) return;

            _isIn = true;
            var real = e.NewValue as string;
            if (real != null && real.Contains(':'))
            {
                var vals = real.Split(new[] {':'}, 2);
                real = vals[1];

                var command = Find(vals[0]);
                if (command != null) SetTargetCommand(d, command);
            }
            else if (GetTargetCommand(d) == null && real != null)
            {
                var command = Find(real);
                if (command != null) SetTargetCommand(d, command);
            }

            foreach (var linker in LinkerCollection.Where(linker => Equals(linker.Target, d)))
            {
                linker.CommandTarget = real;
                linker.Bind();
                return;
            }

            var newlinker = new CommandLinker(d) {CommandTarget = real};
            LinkerCollection.Add(newlinker);
            newlinker.Bind();
            _isIn = false;
        }

        private static void OnCommandStadeChanged([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (_isIn) return;

            foreach (var linker in LinkerCollection.Where(linker => Equals(linker.Target, d)))
            {
                linker.Bind();
                return;
            }
        }

        #endregion
    }

    /// <summary>
    ///     Makiert eine Methode, Property oder feld als Ziel für ein Commando
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess",
        Justification = "Reviewed. Suppression is OK here.")]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
    [PublicAPI]
    [MeansImplicitUse(ImplicitUseKindFlags.Access)]
    public sealed class CommandTargetAttribute : MemberInfoAttribute
    {
        #region Public Properties

        /// <summary>Gets or sets the can execute member.</summary>
        [CanBeNull]
        public string CanExecuteMember { get; set; }

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandTargetAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="CommandTargetAttribute" /> Klasse. Initialisiert eine neue Instanz
        ///     der Klasse.
        /// </summary>
        /// <param name="memberName">
        ///     Der Name des Members.
        /// </param>
        public CommandTargetAttribute([NotNull] string memberName)
            : base(memberName)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandTargetAttribute" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="CommandTargetAttribute" /> Klasse.
        /// </summary>
        public CommandTargetAttribute()
            : base(null)
        {
        }

        #endregion
    }
}