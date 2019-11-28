using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JetBrains.Annotations;
using Tauron.Application.Deployment.AutoUpload.Core.Helper;

namespace Tauron.Application.Deployment.AutoUpload.Core
{
    [PublicAPI]
    public static class CommandBinder
    {
        public static bool AutoRegister { get; set; }
        
        private class CommandLinker : PipelineBase
        {
            public CommandLinker([NotNull] DependencyObject element)
                : base(element, false) { }
            
            [CanBeNull]
            public string CommandTarget { get; set; }
            
            private class CommandFactory
            {
                private bool _isSync;
                
                public CommandFactory([NotNull] WeakReference target, [NotNull] string name)
                {
                    Target = Argument.NotNull(target, nameof(target));
                    Name = Argument.NotNull(name, nameof(name));
                }
                
                private class ParameterMapper
                {
                    public ParameterMapper([NotNull] MethodInfo method, [NotNull] object firstArg)
                    {
                        _method = Argument.NotNull(method, nameof(method));
                        _firstArg = Argument.NotNull(firstArg, nameof(firstArg));
                        _isParameter = method.GetParameters().Length == 1;
                        _isAsync = method.ReturnType.IsAssignableFrom(typeof(Task));
                    }
                    
                    private readonly object _firstArg;

                    private readonly bool _isParameter;

                    private readonly MethodInfo _method;

                    private readonly bool _isAsync;
                    
                    public void CanExecute([NotNull] object sender, [NotNull] CanExecuteRoutedEventArgs e)
                    {
                        Argument.NotNull(sender, nameof(sender));
                        Argument.NotNull(e, nameof(e));

                        var args = _isParameter ? new[] {e.Parameter} : new object[0];
                        e.CanExecute = _isAsync ? _method.InvokeFast<Task<bool>>(_firstArg, args).Result : _method.InvokeFast<bool>(_firstArg, args);
                    }

                    public void Execute([NotNull] object sender, [NotNull] ExecutedRoutedEventArgs e)
                    {
                        Argument.NotNull(sender, nameof(sender));
                        Argument.NotNull(e, nameof(e));

                        _method.InvokeFast(_firstArg, _isParameter ? new[] { e.Parameter } : new object[0]);
                    }

                }

                private class TaskFactory
                {
                    public TaskFactory([NotNull] Delegate del, [NotNull] ITaskScheduler scheduler, bool sync)
                    {
                        _del = Argument.NotNull(del, nameof(del));
                        _scheduler = Argument.NotNull(scheduler, nameof(scheduler));
                        _sync = sync;
                    }
                    
                    public void Handler([NotNull] object parm1, [NotNull] object parm2) => _scheduler.QueueTask(new UserTask(() => _del.DynamicInvoke(parm1, parm2), _sync));

                    private readonly Delegate _del;
                    private readonly ITaskScheduler _scheduler;
                    private readonly bool _sync;
                    
                }

                private class MemberInfoHelper
                {
                    public static readonly MethodInfo CanExecuteMethod =
                        typeof(MemberInfoHelper).GetMethod("CanExecute", new[] {typeof(object), typeof(CanExecuteRoutedEventArgs)});

                    private readonly MemberInfo _info;
                    private readonly object _target;

                    public MemberInfoHelper([NotNull] MemberInfo info, [NotNull] object target)
                    {
                        _info = info;
                        _target = target;
                    }

                    [UsedImplicitly]
                    public void CanExecute([NotNull] object sender, [NotNull] CanExecuteRoutedEventArgs e) => e.CanExecute = _info.GetInvokeMember<bool>(_target);
                }
                
                [CanBeNull]
                public ICommand LastCommand { get; private set; }
                
                [NotNull]
                public string Name { private get; set; }
                
                [CanBeNull]
                public WeakReference Target { private get; set; }
                
                public static void Free([CanBeNull] ICommand command, [NotNull] DependencyObject targetObject)
                {
                    Argument.NotNull(targetObject, nameof(targetObject));

                    var window = Window.GetWindow(targetObject);
                    if (window == null || command == null) return;

                    for (var i = 0; i < window.CommandBindings.Count; i++)
                    {
                        if (window.CommandBindings[i].Command != command) continue;

                        window.CommandBindings.RemoveAt(i);
                        return;
                    }
                }
                
                public void Connect([NotNull] ICommand command, [NotNull] DependencyObject targetObject, [NotNull] ITaskScheduler scheduler, [NotNull] string commandName)
                {
                    Argument.NotNull(scheduler, nameof(scheduler));

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
                    {
                        del = Delegate.CreateDelegate(typeof(ExecutedRoutedEventHandler), target, pair.Item1, false)
                                  .As<ExecutedRoutedEventHandler>() ?? new ParameterMapper(pair.Item1, target).Execute;
                    }

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
                                .As<CanExecuteRoutedEventHandler>() ?? new ParameterMapper(method, localTarget).CanExecute;
                    }

                    if (del != null) binding.Executed += new TaskFactory(del, scheduler, _isSync).Handler;
                    else CommonWpfConstans.LogCommon(false, "CommandBinder: No Compatible method Found: {0}", commandName);

                    if (del2 != null)
                        binding.CanExecute += del2;
                }
                
                [CanBeNull]
                public ICommand GetCommand() => LastCommand;

                [CanBeNull]
                private static CommandBinding SetCommandBinding([CanBeNull] DependencyObject obj, [CanBeNull] ICommand command)
                {
                    while (obj != null && !(obj is Window || obj is UserControl || GetCommandScope(obj)))
                    {
                        var temp = new FrameworkObject(obj, false);
                        obj = temp.Parent ?? temp.VisualParent;
                    }

                    if (obj == null || command == null) return null;

                    var commandBindings = ((UIElement) obj).CommandBindings;

                    var binding = commandBindings.OfType<CommandBinding>().FirstOrDefault(cb => cb.Command == command);
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
                    foreach (var m in targetType.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
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
            }
            
            private class PropertySearcher
            {
                public PropertySearcher([NotNull] WeakReference<DependencyObject> target, [NotNull] string customName,
                    [NotNull] ICommand command)
                {
                    Target = Argument.NotNull(target, nameof(target));
                    CustomName = Argument.NotNull(customName, nameof(customName));
                    Command = Argument.NotNull(command, nameof(command));

                    _changedFlags = PropertyFlags.All;
                }
                
                [NotNull]
                private WeakReference<DependencyObject> Target { get; }
                
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

                                    CommonWpfConstans.LogCommon(false, "CommandBinder: FoundetProperty Incompatible: {0}:{1}", typeName, propName);
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

                [Flags]
                private enum PropertyFlags
                {
                    None = 0,
                    CustomName = 1,
                    Command = 2,
                    All = 3
                }
                
                private PropertyFlags _changedFlags;
                private WeakReference<ICommand> _command;
                private string _customName;
                private PropertyInfo _prop;
                
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
            }
            
            private CommandFactory _factory;
            private PropertySearcher _searcher;
            
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
                    _factory = new CommandFactory(DataContext, commandTarget);
                else
                {
                    _factory.Name = commandTarget;
                    _factory.Target = DataContext;
                }

                if (!useDirect) _factory.Connect(targetCommand, target, Argument.CheckResult(TaskScheduler, "TaskScheduler not set"), commandTarget);

                if (_searcher == null)
                    _searcher = new PropertySearcher(Source ?? throw new InvalidOperationException(), customProperty, targetCommand);
                else
                {
                    _searcher.CustomName = customProperty;
                    _searcher.Command = _factory.GetCommand();
                }

                _searcher.SetCommand();
            }
            
            public void ResetCommand([NotNull] ICommand command)
            {
                var target = Target;
                if (target == null) return;

                SetTargetCommand(target, command);
            }
            
            protected override void DataContextChanged() => Bind();

            private void Free()
            {
                if (_factory != null && Target != null)
                    CommandFactory.Free(_factory.LastCommand, Target);
            }
            
        }

        public static readonly DependencyProperty CommandProperty = 
            DependencyProperty.RegisterAttached("Command", typeof(string), typeof(CommandBinder), new UIPropertyMetadata(null, OnCommandChanged));

        public static readonly DependencyProperty CommandScopeProperty =
            DependencyProperty.RegisterAttached("CommandScope", typeof(bool), typeof(CommandBinder), new PropertyMetadata(false));

        public static readonly DependencyProperty CustomPropertyNameProperty =
            DependencyProperty.RegisterAttached("CustomPropertyName", typeof(string), typeof(CommandBinder), new UIPropertyMetadata("Command", OnCommandStadeChanged));

        public static readonly DependencyProperty TargetCommandProperty =
            DependencyProperty.RegisterAttached("TargetCommand", typeof(ICommand), typeof(CommandBinder), new UIPropertyMetadata(null, OnCommandStadeChanged));

        public static readonly DependencyProperty UseDirectProperty = DependencyProperty.RegisterAttached(
            "UseDirect", typeof(bool), typeof(CommandBinder), new UIPropertyMetadata(false, OnCommandStadeChanged));

        private static readonly List<RoutedCommand> Commands = new List<RoutedCommand>();

        private static readonly WeakReferenceCollection<CommandLinker> LinkerCollection = new WeakReferenceCollection<CommandLinker>();

        private static bool _isIn;
        
        [CanBeNull]
        public static RoutedCommand Find([NotNull] string name)
        {
            var val = Commands.Find(com => com.Name == name);
            if (val == null && AutoRegister) val = Register(name, name);

            return val;
        }
        
        [NotNull]
        public static string GetCommand([NotNull] DependencyObject obj) => (string) Argument.NotNull(obj, nameof(obj)).GetValue(CommandProperty);

        public static bool GetCommandScope([NotNull] DependencyObject obj) => (bool) Argument.NotNull(obj, nameof(obj)).GetValue(CommandScopeProperty);

        [NotNull]
        public static string GetCustomPropertyName([NotNull] DependencyObject obj) => (string) Argument.NotNull(obj, nameof(obj)).GetValue(CustomPropertyNameProperty);

        [CanBeNull]
        public static ICommand GetTargetCommand([NotNull] DependencyObject obj) => (ICommand)Argument.NotNull(obj, nameof(obj)).GetValue(TargetCommandProperty);

        public static bool GetUseDirect([NotNull] DependencyObject obj) => (bool)Argument.NotNull(obj, nameof(obj)).GetValue(UseDirectProperty);

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
        
        [NotNull]
        public static RoutedUICommand Register([NotNull] string text, [NotNull] string name)
        {
            var command = new RoutedUICommand(text, name, typeof(CommandBinder));
            Register(command);
            return command;
        }
        
        public static void SetCommand([NotNull] DependencyObject obj, [NotNull] string value) => Argument.NotNull(obj, nameof(obj)).SetValue(CommandProperty, value);

        public static void SetCommandScope([NotNull] DependencyObject obj, bool value) => Argument.NotNull(obj, nameof(obj)).SetValue(CommandScopeProperty, value);

        public static void SetCustomPropertyName([NotNull] DependencyObject obj, [NotNull] string value) => Argument.NotNull(obj, nameof(obj)).SetValue(CustomPropertyNameProperty, value);

        public static void SetTargetCommand([NotNull] DependencyObject obj, [NotNull] ICommand value) => Argument.NotNull(obj, nameof(obj)).SetValue(TargetCommandProperty, value);

        public static void SetUseDirect([NotNull] DependencyObject obj, bool value) => Argument.NotNull(obj, nameof(obj)).SetValue(UseDirectProperty, value);

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
        
    }
}