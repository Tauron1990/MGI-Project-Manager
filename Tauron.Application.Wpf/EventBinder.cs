using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Catel.IoC;
using Catel.Services;
using JetBrains.Annotations;
using Tauron.Application.Wpf.Commands;
using Tauron.Application.Wpf.Helper;

namespace Tauron.Application.Wpf
{
    [PublicAPI]
    public static class EventBinder
    {
        private const string EventBinderPrefix = "EventBinder.";

        public static readonly DependencyProperty EventsProperty =
            DependencyProperty.RegisterAttached("Events", typeof(string), typeof(EventBinder), new UIPropertyMetadata(null, OnEventsChanged));

        [NotNull]
        public static string GetEvents(DependencyObject obj)
        {
            return (string) Argument.NotNull(obj, nameof(obj)).GetValue(EventsProperty);
        }

        public static void SetEvents(DependencyObject obj, string value)
        {
            Argument.NotNull(obj, nameof(obj)).SetValue(EventsProperty, Argument.NotNull(value, nameof(value)));
        }

        private static void OnEventsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d)) return;

            var root = ControlLogic.FindRoot(d);
            if (root == null)
            {
                if (d is FrameworkElement element)
                    ControlLogic.MakeLazy(element, e.NewValue as string, e.OldValue as string, BindInternal);
                return;
            }

            BindInternal(e.OldValue as string, e.NewValue as string, root, d);
        }

        private static void BindInternal(string? oldValue, string? newValue, IBinderControllable binder, DependencyObject affectedPart)
        {
            if (oldValue != null)
                binder.CleanUp(EventBinderPrefix + oldValue);

            if (newValue == null) return;

            binder.Register(EventBinderPrefix + newValue, new EventLinker {Commands = newValue}, affectedPart);
        }

        private sealed class EventLinker : ControlBindableBase
        {
            private readonly List<InternalEventLinker> _linkers = new List<InternalEventLinker>();

            public string? Commands { get; set; }

            protected override void CleanUp()
            {
                foreach (var linker in _linkers) linker.Dispose();
                _linkers.Clear();
            }

            protected override void Bind(object context)
            {
                if (Commands == null)
                {
                    Debug.Print("EventBinder: No Command Setted: {0}", context ?? "Unkowen");

                    return;
                }

                var vals = Commands.Split(':');
                var events = new Dictionary<string, string>();

                try
                {
                    for (var i = 0; i < vals.Length; i++) events[vals[i]] = vals[++i];
                }
                catch (IndexOutOfRangeException)
                {
                    Debug.Print("EventBinder: EventPairs not Valid: {0}", Commands);
                }

                if (context == null) return;

                var dataContext = context;
                var host = AffectedObject;
                if (host == null || dataContext == null) return;

                var hostType = host.GetType();
                var dataContextInfos =
                    (from entry in dataContext.GetType().FindMemberAttributes<EventTargetAttribute>(true)
                        select new CommandMember(
                            entry.Item2.ProvideMemberName(entry.Item1),
                            entry.Item1,
                            entry.Item2.Synchronize,
                            entry.Item2.Converter)).ToArray();

                foreach (var pair in events)
                {
                    var info = hostType.GetEvent(pair.Key);
                    if (info == null)
                    {
                        Debug.Print("EventBinder: No event Found: {0}|{1}", hostType, pair.Key);
                        return;
                    }

                    _linkers.Add(new InternalEventLinker(dataContextInfos, info, context, pair.Value, host));
                }
            }

            private class CommandMember
            {
                public CommandMember(string name, MemberInfo memberInfo, bool synchronize, Type? converter)
                {
                    Name = name;
                    MemberInfo = memberInfo;
                    Synchronize = synchronize;
                    Converter = converter;
                }

                public MemberInfo MemberInfo { get; }

                public string Name { get; }

                public bool Synchronize { get; }

                public Type? Converter { get; }
            }

            private class InternalEventLinker : IDisposable
            {
                private static readonly MethodInfo Method = typeof(InternalEventLinker).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                    .First(m => m.Name == "Handler");

                private readonly object _dataContext;

                private readonly EventInfo? _event;
                private readonly DependencyObject? _host;
                private readonly string _targetName;
                private InvokeHelper? _command;
                private Delegate? _delegate;
                private bool _isDirty;
                private MemberInfo? _member;
                private TypeConverter? _simpleConverter;
                private bool _sync;

                public InternalEventLinker(IEnumerable<CommandMember>? member, EventInfo? @event, object dataContext,
                    [NotNull] string targetName, DependencyObject? host)
                {
                    _isDirty = (member == null) | (@event == null);

                    _host = host;
                    _event = @event;
                    _dataContext = dataContext;
                    _targetName = targetName;
                    FindMember(member);

                    Initialize();
                }

                public void Dispose()
                {
                    if (_host == null || _delegate == null) return;

                    _event?.RemoveEventHandler(_host, _delegate);
                    _delegate = null;
                }

                private bool EnsureCommandStade()
                {
                    if (_command != null) return true;

                    if (_member == null) return false;

                    try
                    {
                        if (_dataContext == null) return false;

                        var minfo = _member as MethodInfo;
                        if (minfo != null)
                            _command = new InvokeHelper(minfo, _dataContext);
                    }
                    catch (InvalidCastException e)
                    {
                        Debug.Print("EventBinder: Casting Faild: {0}|{1}|{2}", _dataContext, _targetName, e);
                        _isDirty = true;
                    }
                    catch (ArgumentException e)
                    {
                        Debug.Print("EventBinder: invalid Argument: {0}|{1}|{2}", _dataContext, _targetName, e);

                        _isDirty = true;
                    }

                    return _command != null && !_isDirty;
                }

                private void FindMember(IEnumerable<CommandMember>? members)
                {
                    if (members == null)
                    {
                        Debug.Print("EventBinder: No Members: {0}", _dataContext);
                        return;
                    }

                    var temp = members.FirstOrDefault(mem => mem.Name == _targetName);
                    if (temp == null)
                    {
                        Debug.Print("EventBinder: No Valid Member found: {0}|{1}", _dataContext, _targetName);
                        return;
                    }

                    _member = temp.MemberInfo;
                    _sync = temp.Synchronize;
                    try
                    {
                        _simpleConverter = temp.Converter != null ? temp.Converter.FastCreateInstance() as TypeConverter : null;
                    }
                    catch (Exception e)
                    {
                        Debug.Print($"Error Bind Event: {e.GetType()}--{e.Message}");
                    }
                }

                [UsedImplicitly]
                private async void Handler(object sender, EventArgs e)
                {
                    if (!_isDirty && !EnsureCommandStade())
                    {
                        Dispose();
                        return;
                    }

                    try
                    {
                        var localSender = _simpleConverter?.ConvertFrom(sender) ?? sender;
                        var localEventArgs = _simpleConverter?.ConvertFrom(e) ?? e;

                        var data = new EventData(localSender, localEventArgs);

                        await (_command?.Execute(data, _sync) ?? Task.CompletedTask).ConfigureAwait(false);
                    }
                    catch (ArgumentException)
                    {
                        _isDirty = true;
                    }
                }

                private void Initialize()
                {
                    if (_isDirty || _event == null) return;

                    var eventTyp = _event?.EventHandlerType;
                    if (_host == null || eventTyp == null) return;

                    _delegate = Delegate.CreateDelegate(eventTyp, this, Method);
                    _event?.AddEventHandler(_host, _delegate);
                }

                private sealed class InvokeHelper
                {
                    private static readonly IDispatcherService DispatcherService = DependencyResolverManager.Default.DefaultDependencyResolver.Resolve<IDispatcherService>();

                    private readonly MethodInfo _method;
                    private readonly MethodType _methodType;

                    public InvokeHelper(MethodInfo method, object context)
                    {
                        _method = Argument.NotNull(method, nameof(method));
                        Context = Argument.NotNull(context, nameof(context));

                        _methodType = (MethodType) method.GetParameters().Length;
                        if (_methodType != MethodType.One) return;
                        if (_method.GetParameters()[0].ParameterType != typeof(EventData)) _methodType = MethodType.EventArgs;
                    }

                    private object? Context { get; }

                    public async Task Execute(EventData parameter, bool sync)
                    {
                        var args = _methodType switch
                        {
                            MethodType.Zero => new object[0],
                            MethodType.One => new object[] {parameter},
                            MethodType.Two => new[] {parameter?.Sender, parameter?.EventArgs},
                            MethodType.EventArgs => new[] {parameter?.EventArgs},
                            _ => new object[0]
                        };

                        if (sync)
                            await DispatcherService.InvokeAsync(() => _method.InvokeFast(Context, args)).ConfigureAwait(false);
                        else
                            await (_method.InvokeFast<Task>(Context, args) ?? Task.CompletedTask).ConfigureAwait(false);
                    }

                    private enum MethodType
                    {
                        Zero = 0,
                        One,
                        Two,
                        EventArgs
                    }
                }
            }
        }
    }
}