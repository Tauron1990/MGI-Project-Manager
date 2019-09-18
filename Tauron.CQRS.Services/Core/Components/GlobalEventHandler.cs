using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using CQRSlite.Events;
using JetBrains.Annotations;

namespace Tauron.CQRS.Services.Core.Components
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public sealed class GlobalEventHandler<TMessage> : IEventHandler<TMessage> where TMessage : IEvent
    {
        private abstract class WeakAction<T> where T : class
        {
            private readonly MethodInfo _method;

            private class WeakActionStatic : WeakAction<T>
            {
                private readonly Type _Owner;

                public WeakActionStatic(MethodInfo method)
                    : base(method)
                {
                    if (!method.IsStatic)
                    {
                        throw new ArgumentException("static method expected", "method");
                    }
                    _Owner = method.DeclaringType;
                }

                public override bool HasBeenCollected
                {
                    get { return false; }
                }

                protected override T GetMethod()
                {
                    return System.Delegate.CreateDelegate(typeof(T), _Owner, _method.Name) as T;
                }
            }

            private class WeakActionInstance : WeakAction<T>
            {
                private readonly WeakReference _WeakRef;

                protected WeakActionInstance(MethodInfo method)
                    : base(method)
                {
                }

                public WeakActionInstance(object instance, MethodInfo method)
                    : this(method)
                {
                    if (instance == null)
                    {
                        throw new ArgumentNullException("instance must not be null", "instance");
                    }
                    _WeakRef = new WeakReference(instance);
                }

                public override bool HasBeenCollected
                {
                    get { return !_WeakRef.IsAlive; }
                }

                protected override T GetMethod()
                {
                    if (HasBeenCollected) { return null; }

                    object localTarget = _WeakRef.Target;
                    if (localTarget == null) { return null; }

                    return System.Delegate.CreateDelegate(typeof(T), localTarget, _method.Name) as T;
                }
            }

            protected WeakAction(MethodInfo method)
            {
                _method = method;
            }

            public static WeakAction<T> Create(Expression expression)
            {
                return new WeakActionStatic(GetMethodInfo(expression));
            }

            public static WeakAction<T> Create(object instance, Expression expression)
            {
                return new WeakActionInstance(instance, GetMethodInfo(expression));
            }

            protected abstract T GetMethod();

            public T Delegate
            {
                get { return GetMethod(); }
            }

            public abstract bool HasBeenCollected { get; }

            public bool IsInvokable
            {
                get { return !HasBeenCollected; }
            }

            private static MethodInfo GetMethodInfo(Expression expression)
            {
                LambdaExpression lambda = expression as LambdaExpression;
                if (lambda == null)
                {
                    throw new ArgumentException("expression is not LambdaExpression");
                }

                MethodCallExpression outermostExpression = lambda.Body as MethodCallExpression;

                if (outermostExpression == null)
                {
                    throw new ArgumentException("Invalid Expression. Expression should consist of a Method call only.");
                }

                return outermostExpression.Method;
            }
        }

        private class RegisterDispose : IDisposable
        {
            private readonly GlobalEventHandler<TMessage> _handler;
            private readonly object _key;

            public RegisterDispose(GlobalEventHandler<TMessage> handler, object key)
            {
                _handler = handler;
                _key = key;
            }

            public void Dispose() => _handler._handlerRegistry.Remove(_key);
        }

        private readonly Dictionary<object, WeakAction<Func<TMessage, Task>>> _handlerRegistry = new Dictionary<object, WeakAction<Func<TMessage, Task>>>();

        //public IDisposable Register(Func<TMessage, Task> awaiter)
        //{
        //    var key = new object();

        //    _handlerRegistry[key] = awaiter;

        //    return new RegisterDispose(this, key);
        //}

        public IDisposable Register(object instance, Expression<Func<TMessage, Task>> awaiter)
        {
            var key = new object();

            _handlerRegistry[key] = WeakAction<Func<TMessage, Task>>.Create(instance, awaiter);

            return new RegisterDispose(this, key);
        }

        public async Task Handle(TMessage message)
        {
            foreach (var task in _handlerRegistry.Values
                .Select(handlerRegistryValue => handlerRegistryValue.Delegate?.Invoke(message))
                .Where(t => t != null)) 
                await task;
        }
    }
}