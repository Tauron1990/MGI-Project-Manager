using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Catel.IoC;
using Catel.Logging;
using Catel.Services;
using JetBrains.Annotations;
using Tauron.Application.Wpf.Helper;

namespace Tauron.Application.Wpf
{
    [PublicAPI]
    public static class CommandBinder
    {
        private const string NamePrefix = "CommandBinder.";

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(string), typeof(CommandBinder), new UIPropertyMetadata(null, OnCommandChanged));

        public static readonly DependencyProperty CustomPropertyNameProperty =
            DependencyProperty.RegisterAttached("CustomPropertyName", typeof(string), typeof(CommandBinder), new UIPropertyMetadata("Command", OnCommandStadeChanged));

        public static readonly DependencyProperty TargetCommandProperty =
            DependencyProperty.RegisterAttached("TargetCommand", typeof(ICommand), typeof(CommandBinder), new UIPropertyMetadata(null, OnCommandStadeChanged));

        public static readonly DependencyProperty UseDirectProperty = DependencyProperty.RegisterAttached(
            "UseDirect", typeof(bool), typeof(CommandBinder), new UIPropertyMetadata(false, OnCommandStadeChanged));

        private static readonly List<RoutedCommand> Commands = new List<RoutedCommand>();

        private static bool _isIn;

        public static bool AutoRegister { get; set; } = true;

        public static RoutedCommand? Find(string name)
        {
            var val = Commands.Find(com => com.Name == name);
            if (val == null && AutoRegister) val = Register(name, name);

            return val;
        }

        public static string GetCommand(DependencyObject obj) => (string) Argument.NotNull(obj, nameof(obj)).GetValue(CommandProperty);


        public static string GetCustomPropertyName(DependencyObject obj) => (string) Argument.NotNull(obj, nameof(obj)).GetValue(CustomPropertyNameProperty);

        public static ICommand? GetTargetCommand(DependencyObject obj) => (ICommand) Argument.NotNull(obj, nameof(obj)).GetValue(TargetCommandProperty);

        public static bool GetUseDirect(DependencyObject obj) => (bool) Argument.NotNull(obj, nameof(obj)).GetValue(UseDirectProperty);

        public static void Register(RoutedCommand command)
        {
            if (Commands.Any(com => com.Name == command.Name)) return;

            Commands.Add(command);
        }

        public static RoutedUICommand Register(string text, string name)
        {
            var command = new RoutedUICommand(text, name, typeof(CommandBinder));
            Register(command);
            return command;
        }

        public static void SetCommand(DependencyObject obj, string value)
        {
            Argument.NotNull(obj, nameof(obj)).SetValue(CommandProperty, value);
        }

        public static void SetCustomPropertyName(DependencyObject obj, string value)
        {
            Argument.NotNull(obj, nameof(obj)).SetValue(CustomPropertyNameProperty, value);
        }

        public static void SetTargetCommand(DependencyObject obj, ICommand value)
        {
            Argument.NotNull(obj, nameof(obj)).SetValue(TargetCommandProperty, value);
        }

        public static void SetUseDirect(DependencyObject obj, bool value)
        {
            Argument.NotNull(obj, nameof(obj)).SetValue(UseDirectProperty, value);
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d)) return;

            var root = ControlLogic.FindRoot(d);
            if (root == null)
            {
                if (!(d is FrameworkElement element)) return;

                ControlLogic.MakeLazy(element, e.NewValue as string, e.OldValue as string, BindInternal);
                return;
            }

            BindInternal(e.OldValue as string, e.NewValue as string, root, d);
        }

        private static void OnCommandStadeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (_isIn) return;

            var root = ControlLogic.FindRoot(d);
            if (root == null) return;

            var name = GetCommand(d);
            BindInternal(name, name, root, d);
        }

        private static void BindInternal(string? oldValue, string? newValue, IBinderControllable binder, DependencyObject affectedPart)
        {
            _isIn = true;

            if (oldValue != null)
                binder.CleanUp(NamePrefix + oldValue);
            if (newValue == null) return;

            var name = NamePrefix + newValue;
            if (newValue != null && newValue.Contains(':'))
            {
                var vals = newValue.Split(new[] {':'}, 2);
                newValue = vals[1];

                var command = Find(vals[0]);
                if (command != null) SetTargetCommand(affectedPart, command);
            }
            else if (GetTargetCommand(affectedPart) == null && newValue != null)
            {
                var command = Find(newValue);
                if (command != null) SetTargetCommand(affectedPart, command);
            }
            else
            {
                var command = GetTargetCommand(affectedPart);
                if (command is RoutedCommand routedCommand)
                    name = NamePrefix + routedCommand.Name;
                else
                    name = NamePrefix + command;
            }

            var newlinker = new CommandLinker {CommandTarget = newValue};
            binder.Register(name, newlinker, affectedPart);
            _isIn = false;
        }

        private class CommandLinker : ControlBindableBase
        {
            private CommandFactory? _factory;
            private PropertySearcher? _searcher;

            public string? CommandTarget { get; set; }

            protected override void CleanUp()
            {
                _factory?.Free(Root);
            }

            protected override void Bind(object dataContext)
            {
                var commandTarget = CommandTarget;
                if (commandTarget == null)
                {
                    Debug.Print("CommandBinder: No Binding: CommandTarget");
                    return;
                }

                var customProperty = GetCustomPropertyName(AffectedObject);
                var useDirect = GetUseDirect(AffectedObject);
                var targetCommand = GetTargetCommand(AffectedObject);
                if (targetCommand == null)
                {
                    Debug.Print($"CommandBinder: No ICommand: {commandTarget}");
                    return;
                }

                if (_factory == null)
                {
                    _factory = new CommandFactory(dataContext, commandTarget);
                }
                else
                {
                    _factory.Name = commandTarget;
                    _factory.DataContext = dataContext;
                }

                if (!useDirect)
                    _factory.Connect(targetCommand, Root, commandTarget);

                if (_searcher == null)
                {
                    _searcher = new PropertySearcher(AffectedObject, customProperty, targetCommand);
                }
                else
                {
                    _searcher.CustomName = customProperty;
                    _searcher.Command = _factory.LastCommand;
                }

                _searcher.SetCommand();
            }

            private class CommandFactory
            {
                private CanExecuteRoutedEventHandler? _canExecute;

                private ExecutedRoutedEventHandler? _execute;
                private bool _isSync;

                public CommandFactory(object dataContext, string name)
                {
                    DataContext = Argument.NotNull(dataContext, nameof(dataContext));
                    Name = Argument.NotNull(name, nameof(name));
                }

                public ICommand? LastCommand { get; private set; }

                public string Name { private get; set; }

                public object DataContext { private get; set; }

                public void Free([NotNull] DependencyObject rootObject)
                {
                    Argument.NotNull(rootObject, nameof(rootObject));

                    if (!(rootObject is UIElement uiElement)) return;

                    var commandBinding = uiElement.CommandBindings.OfType<CommandBinding>().FirstOrDefault(cb => cb.Command == LastCommand);
                    if (commandBinding == null) return;

                    if (_execute != null)
                        commandBinding.Executed -= _execute;
                    if (_canExecute != null)
                        commandBinding.CanExecute -= _canExecute;

                    _execute = null;
                    _canExecute = null;

                    uiElement.CommandBindings.Remove(commandBinding);
                }

                public void Connect(ICommand command, DependencyObject rootObject, string commandName)
                {
                    var targetType = DataContext.GetType();

                    LastCommand = command;

                    var pair = FindCommandPair(targetType, out var ok);

                    var temp = command as RoutedCommand;
                    var binding = SetCommandBinding(rootObject, temp);

                    if (binding == null || !ok || pair == null) return;


                    Func<object, ExecutedRoutedEventArgs, Task>? del = null;
                    if (pair.Item1 != null)
                        del = Delegate.CreateDelegate(typeof(Func<object, ExecutedRoutedEventArgs, Task>), DataContext, pair.Item1, false)
                            as Func<object, ExecutedRoutedEventArgs, Task> ?? new ParameterMapper(pair.Item1, DataContext).Execute;

                    CanExecuteRoutedEventHandler? del2 = null;
                    if (pair.Item2 != null)
                    {
                        var method = pair.Item2 as MethodInfo;
                        var localTarget = DataContext;

                        if (method == null)
                        {
                            method = MemberInfoHelper.CanExecuteMethod;
                            localTarget = new MemberInfoHelper(pair.Item2, DataContext);
                        }

                        del2 =
                            Delegate.CreateDelegate(typeof(CanExecuteRoutedEventHandler), localTarget, method, false)
                                as CanExecuteRoutedEventHandler ?? new ParameterMapper(method, localTarget).CanExecute;
                    }

                    if (del != null)
                    {
                        _execute = new TaskFactory(del, _isSync).Handler;
                        binding.Executed += _execute;
                    }
                    else
                    {
                        Debug.Print($"CommandBinder: No Compatible method Found: {commandName}");
                    }

                    if (del2 == null) return;

                    _canExecute = del2;
                    binding.CanExecute += del2;
                }

                private static CommandBinding? SetCommandBinding(DependencyObject? obj, ICommand? command)
                {
                    var commandBindings = (obj as UIElement)?.CommandBindings;
                    if (commandBindings == null) return null;

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

                private class ParameterMapper
                {
                    private readonly object _firstArg;

                    private readonly bool _isAsync;

                    private readonly bool _isParameter;

                    private readonly MethodInfo _method;

                    public ParameterMapper(MethodInfo method, object firstArg)
                    {
                        _method = Argument.NotNull(method, nameof(method));
                        _firstArg = Argument.NotNull(firstArg, nameof(firstArg));
                        _isParameter = method.GetParameters().Length == 1;
                        _isAsync = method.ReturnType.IsAssignableFrom(typeof(Task));
                    }

                    public void CanExecute([NotNull] object sender, [NotNull] CanExecuteRoutedEventArgs e)
                    {
                        Argument.NotNull(sender, nameof(sender));
                        Argument.NotNull(e, nameof(e));

                        var args = _isParameter ? new[] {e.Parameter} : new object[0];
                        e.CanExecute = _isAsync ? _method.InvokeFast<Task<bool>>(_firstArg, args)?.Result ?? true : _method.InvokeFast<bool>(_firstArg, args);
                    }

                    public async Task Execute([NotNull] object sender, [NotNull] ExecutedRoutedEventArgs e)
                    {
                        Argument.NotNull(sender, nameof(sender));
                        Argument.NotNull(e, nameof(e));

                        if (_isAsync)
                            await (_method.InvokeFast<Task>(_firstArg, _isParameter ? new[] {e.Parameter} : new object[0]) ?? Task.CompletedTask);
                        else
                            _method.InvokeFast(_firstArg, _isParameter ? new[] {e.Parameter} : new object[0]);
                    }
                }

                private class TaskFactory
                {
                    private static readonly Dispatcher DispatcherService = System.Windows.Application.Current.Dispatcher;

                    private readonly Func<object, ExecutedRoutedEventArgs, Task> _del;
                    private readonly bool _sync;

                    public TaskFactory(Func<object, ExecutedRoutedEventArgs, Task> del, bool sync)
                    {
                        _del = Argument.NotNull(del, nameof(del));
                        _sync = sync;
                    }

                    public async void Handler(object parm1, ExecutedRoutedEventArgs parm2)
                    {
                        try
                        {
                            if (_sync)
                                await DispatcherService.InvokeAsync(async () => await _del(parm1, parm2));
                            else
                                await _del(parm1, parm2).ConfigureAwait(false);
                        }
                        catch (Exception e)
                        {
                            LogManager.GetLogger<CommandFactory>().Error(e);
                        }
                    }
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
                    public void CanExecute([NotNull] object sender, [NotNull] CanExecuteRoutedEventArgs e)
                    {
                        e.CanExecute = _info.GetInvokeMember<bool>(_target);
                    }
                }
            }

            private class PropertySearcher
            {
                private PropertyFlags _changedFlags;
                private WeakReference<ICommand>? _command;
                private string? _customName;
                private PropertyInfo? _prop;

                public PropertySearcher(DependencyObject affectedObject, string customName, ICommand command)
                {
                    AffectedObject = Argument.NotNull(affectedObject, nameof(affectedObject));
                    CustomName = Argument.NotNull(customName, nameof(customName));
                    Command = Argument.NotNull(command, nameof(command));

                    _changedFlags = PropertyFlags.All;
                }

                private DependencyObject AffectedObject { get; }

                public ICommand? Command
                {
                    private get { return _command?.TypedTarget(); }
                    set
                    {
                        if (_command != null && _command.TypedTarget() == value) return;
                        if (value == null)
                        {
                            _command = null;
                        }
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

                public void SetCommand()
                {
                    try
                    {
                        var commandChanged = _changedFlags.HasFlag(PropertyFlags.Command);
                        var customNameChanged = _changedFlags.HasFlag(PropertyFlags.CustomName);

                        if (customNameChanged)
                        {
                            var tarType = AffectedObject.GetType();
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

                        if (commandChanged && _prop != null)
                            _prop.SetInvokeMember(AffectedObject, Command);
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
            }
        }
    }
}