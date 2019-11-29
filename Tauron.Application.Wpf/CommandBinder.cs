using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JetBrains.Annotations;
using Tauron.Application.Wpf.Helper;

namespace Tauron.Application.Wpf
{
    [PublicAPI]
    public static class CommandBinder
    {
        public static bool AutoRegister { get; set; } = true;
        
        private class CommandLinker : ControlBindableBase
        {
            public string? CommandTarget { get; set; }
            
            private class CommandFactory
            {
                private bool _isSync;
                
                public CommandFactory([NotNull] object target, [NotNull] string name)
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

                    public async Task Execute([NotNull] object sender, [NotNull] ExecutedRoutedEventArgs e)
                    {
                        Argument.NotNull(sender, nameof(sender));
                        Argument.NotNull(e, nameof(e));

                        if(_isAsync)
                            await _method.InvokeFast<Task>(_firstArg, _isParameter ? new[] { e.Parameter } : new object[0]);
                        else
                            _method.InvokeFast(_firstArg, _isParameter ? new[] { e.Parameter } : new object[0]);
                    }

                }

                private class TaskFactory
                {
                    public TaskFactory([NotNull] Func<object, ExecutedRoutedEventArgs, Task> del, [NotNull] ITaskScheduler scheduler, bool sync)
                    {
                        _del = Argument.NotNull(del, nameof(del));
                        _scheduler = Argument.NotNull(scheduler, nameof(scheduler));
                        _sync = sync;
                    }
                    
                    public void Handler([NotNull] object parm1, [NotNull] object parm2) => _scheduler.QueueTask(new UserTask(() => _del.DynamicInvoke(parm1, parm2), _sync));

                    private readonly Func<object, ExecutedRoutedEventArgs, Task> _del;
                    private readonly bool _sync;
                    
                }

                private class MemberInfoHelper
                {
                    public static readonly MethodInfo CanExecuteMethod =
                        Argument.CheckResult(typeof(MemberInfoHelper).GetMethod("CanExecute", new[] {typeof(object), typeof(CanExecuteRoutedEventArgs)}), "MemberInfoHelper.CanExecute");

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
                
                public ICommand? LastCommand { get; private set; }
                
                [NotNull]
                public string Name { private get; set; }
                
                public object Target { private get; set; }
                
                public static void Free(ICommand? command, [NotNull] DependencyObject targetObject)
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
                
                public void Connect([NotNull] ICommand command, [NotNull] DependencyObject targetObject, [NotNull] string commandName)
                {
                    var target = Target;
                    if (target == null) return;

                    var targetType = target.GetType();

                    LastCommand = command;

                    var pair = FindCommandPair(targetType, out var ok);

                    var temp = command as RoutedCommand;
                    var binding = SetCommandBinding(targetObject, temp);

                    if (binding == null || !ok || pair == null) return;


                    Func<object, ExecutedRoutedEventArgs, Task>? del = null;
                    if (pair.Item1 != null)
                    {
                        del = Delegate.CreateDelegate(typeof(Func<object, ExecutedRoutedEventArgs, Task>), target, pair.Item1, false)
                                  as Func<object, ExecutedRoutedEventArgs, Task> ?? new ParameterMapper(pair.Item1, target).Execute;
                    }

                    CanExecuteRoutedEventHandler? del2 = null;
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
                                as CanExecuteRoutedEventHandler ?? new ParameterMapper(method, localTarget).CanExecute;
                    }

                    if (del != null) binding.Executed += new TaskFactory(del, scheduler, _isSync).Handler;
                    else Debug.Print($"CommandBinder: No Compatible method Found: {commandName}");

                    if (del2 != null)
                        binding.CanExecute += del2;
                }
                
                public ICommand? GetCommand() => LastCommand;

                private static CommandBinding? SetCommandBinding(DependencyObject? obj, ICommand? command)
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

                private Tuple<MethodInfo, MemberInfo?>? FindCommandPair([NotNull] IReflect targetType, out bool finded)
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
                        Debug.Print($"CommandBinder: No Command-Method Found: {Name}");
                        return null;
                    }

                    finded = true;

                    _isSync = main.IsSync;

                    var mainAttr = main.Method.GetCustomAttribute<CommandTargetAttribute>();
                    MemberInfo? second = null;
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
                public PropertySearcher(DependencyObject target, [NotNull] string customName,
                    [NotNull] ICommand command)
                {
                    Target = Argument.NotNull(target, nameof(target));
                    CustomName = Argument.NotNull(customName, nameof(customName));
                    Command = Argument.NotNull(command, nameof(command));

                    _changedFlags = PropertyFlags.All;
                }
                
                private DependencyObject Target { get; }
                
                public void SetCommand()
                {
                    try
                    {
                        var commandChanged = _changedFlags.HasFlag(PropertyFlags.Command);
                        var customNameChanged = _changedFlags.HasFlag(PropertyFlags.CustomName);

                        if (customNameChanged)
                        {
                            var dependencyObject = Target;
                            if (dependencyObject != null)
                            {
                                var tarType = dependencyObject.GetType();
                                _prop = tarType.GetProperty(CustomName ?? string.Empty);
                                if (_prop != null
                                    && (!_prop.CanWrite || !typeof(ICommand).IsAssignableFrom(_prop.PropertyType)))
                                {
                                    var typeName = tarType.ToString();
                                    var propName = _prop == null ? CustomName + "(Not Found)" : _prop.Name;

                                    Debug.Print("CommandBinder: FoundetProperty Incompatible: {0}:{1}", typeName, propName);
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
                private WeakReference<ICommand>? _command;
                private string? _customName;
                private PropertyInfo? _prop;
                
                public ICommand? Command
                {
                    private get { return _command?.TypedTarget(); }
                    set
                    {
                        if (_command != null && _command.TypedTarget() == value) return;
                        if (value == null)
                            _command = null;
                        else
                        {
                            _command = new WeakReference<ICommand>(value);
                            _changedFlags |= PropertyFlags.Command;
                        }
                    }
                }
                
                public string? CustomName
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
            
            private CommandFactory? _factory;
            private PropertySearcher? _searcher;

            protected override void CleanUp() 
                => Free();

            protected override void Bind(object dataContext)
            {
                var target = AffectedObject;
                var commandTarget = CommandTarget;
                if (commandTarget == null)
                {
                    Debug.Print($"CommandBinder: No Binding: CommandTarget");
                    return;
                }

                var customProperty = GetCustomPropertyName(target);
                var useDirect = GetUseDirect(target);
                var targetCommand = GetTargetCommand(target);
                if (targetCommand == null)
                {
                    Debug.Print($"CommandBinder: No ICommand: {commandTarget}");
                    return;
                }

                if (_factory == null)
                    _factory = new CommandFactory(dataContext, commandTarget);
                else
                {
                    _factory.Name = commandTarget;
                    _factory.Target = dataContext;
                }

                if (!useDirect) _factory.Connect(targetCommand, target, Argument.CheckResult(TaskScheduler, "TaskScheduler not set"), commandTarget);

                if (_searcher == null)
                    _searcher = new PropertySearcher(AffectedObject, customProperty, targetCommand);
                else
                {
                    _searcher.CustomName = customProperty;
                    _searcher.Command = _factory.GetCommand();
                }

                _searcher.SetCommand();
            }
            
            public void ResetCommand([NotNull] ICommand command)
            {
                var target = AffectedObject;

                SetTargetCommand(target, command);
            }
            
            private void Free() 
                => CommandFactory.Free(_factory?.LastCommand, AffectedObject);
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

        private static bool _isIn;
        
        public static RoutedCommand? Find(string name)
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
        }
        
        [NotNull]
        public static RoutedUICommand Register([NotNull] string text, [NotNull] string name)
        {
            var command = new RoutedUICommand(text, name, typeof(CommandBinder));
            Register(command);
            return command;
        }
        
        public static void SetCommand(DependencyObject obj, string value) => Argument.NotNull(obj, nameof(obj)).SetValue(CommandProperty, value);

        public static void SetCommandScope(DependencyObject obj, bool value) => Argument.NotNull(obj, nameof(obj)).SetValue(CommandScopeProperty, value);

        public static void SetCustomPropertyName(DependencyObject obj, string value) => Argument.NotNull(obj, nameof(obj)).SetValue(CustomPropertyNameProperty, value);

        public static void SetTargetCommand(DependencyObject obj, ICommand value) => Argument.NotNull(obj, nameof(obj)).SetValue(TargetCommandProperty, value);

        public static void SetUseDirect(DependencyObject obj, bool value) => Argument.NotNull(obj, nameof(obj)).SetValue(UseDirectProperty, value);

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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