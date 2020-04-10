using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Tauron.Application
{
    [PublicAPI]
    public sealed class WeakAction
    {
        private bool Equals(WeakAction other) => Equals(MethodInfo, other.MethodInfo) && Equals(TargetObject?.Target, other.TargetObject?.Target);

        public override int GetHashCode()
        {
            unchecked
            {
                int? value = TargetObject?.Target.GetHashCode();

                return ((MethodInfo != null ? MethodInfo.GetHashCode() : 0) * 397) ^ (value == null ? 0 : value.Value);
            }
        }
        
        public object? Invoke(params object[] parms)
        {
            var temp = CreateDelegate(out var target);
            return temp?.Invoke(target, parms);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is WeakAction action && Equals(action);
        }
        
        private readonly Type _delegateType;

        private readonly Type?[] _parames;

        public WeakAction(object? target, MethodInfo method, Type? parameterType)
        {
            if (target != null)
                TargetObject = new WeakReference(target);

            MethodInfo = Argument.NotNull(method, nameof(method));
            _parames = new[] {parameterType};

            _delegateType = parameterType == null
                ? typeof(Action)
                : typeof(Action<>).MakeGenericType(parameterType);

            ParameterCount = parameterType == null ? 0 : 1;
        }
        
        public WeakAction([CanBeNull] object target, [NotNull] MethodInfo method)
        {
            MethodInfo = Argument.NotNull(method, nameof(method));
            if (target != null)
                TargetObject = new WeakReference(target);

            _parames = method.GetParameters().OrderBy(parm => parm.Position).Select(parm => parm.ParameterType).ToArray();
            var returntype = method.ReturnType;
            _delegateType = returntype == typeof(void)
                ? FactoryDelegateType("System.Action", _parames.ToArray())
                : FactoryDelegateType("System.Func", _parames.Concat(new[] {returntype}).ToArray());

            ParameterCount = _parames.Length;
        }

        public int ParameterCount { get; private set; }
        
        public MethodInfo MethodInfo { get; }

        public WeakReference? TargetObject { get; }
        
        internal Func<object?, object?[]?, object?>? CreateDelegate(out object? target)
        {
            target = TargetObject?.Target;
            return target != null
                ? MethodInfo.GetMethodInvoker(() => _parames)
                : null;
        }
        
        [NotNull]
        private static Type FactoryDelegateType([NotNull] string name, [NotNull] Type?[] types)
        {
            Argument.NotNull(types, nameof(types));
            Argument.NotNull(name, nameof(name));

            var type = Type.GetType(name + "`" + types.Length);
            if (type != null)
                return types.Length > 0 ? type.MakeGenericType(types) : Argument.CheckResult(Type.GetType(name), "Delegate Type Was Null");

            throw new InvalidOperationException();
        }
        
    }

    [PublicAPI]
    public class WeakActionEvent<T>
    {
        private readonly List<WeakAction> _delegates = new List<WeakAction>();
        
        public WeakActionEvent() => WeakCleanUp.RegisterAction(CleanUp);

        private void CleanUp()
        {
            lock (this)
            {
                if (_delegates == null) return;

                var dead = _delegates.Where(item => item.TargetObject?.IsAlive == false).ToList();

                lock (this)
                    dead.ForEach(ac => _delegates.Remove(ac));
            }
        }
        
        [NotNull]
        public WeakActionEvent<T> Add([NotNull] Action<T> handler)
        {
            Argument.NotNull(handler, nameof(handler));
            var parameters = handler.Method.GetParameters();

            lock (this)
            {
                if (_delegates.Where(del => del.MethodInfo == handler.Method)
                        .Select(weakAction => weakAction.TargetObject?.Target)
                        .Any(weakTarget => weakTarget == handler.Target))
                    return this;
            }

            var parameterType = parameters[0].ParameterType;

            lock (this)
                _delegates.Add(new WeakAction(handler.Target, handler.Method, parameterType));

            return this;
        }
        
        public void Invoke(T arg)
        {
            lock (this)
            {
                foreach (var action in _delegates)
                {
                    var del = action.CreateDelegate(out var target);
                    if (target != null)
                        del?.Invoke(target, new object?[] {arg});
                }
            }
        }
        
        [NotNull]
        public WeakActionEvent<T> Remove([NotNull] Action<T> handler)
        {
            Argument.NotNull(handler, nameof(handler));
            lock (this)
            {
                foreach (var del in _delegates.Where(del => del.TargetObject != null && del.TargetObject.Target == handler.Target))
                {
                    _delegates.Remove(del);
                    return this;
                }
            }

            return this;
        }
    }
}