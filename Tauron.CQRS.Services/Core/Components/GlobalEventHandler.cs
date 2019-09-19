using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using CQRSlite.Events;
using CQRSlite.Messages;
using JetBrains.Annotations;

namespace Tauron.CQRS.Services.Core.Components
{
    public abstract class GlobalEventHandlerBase
    {
        public abstract Task Handle(IMessage message);
    }

    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public sealed class GlobalEventHandler<TMessage> : GlobalEventHandlerBase, IEventHandler<TMessage> where TMessage : IEvent
    {
        private abstract class WeakAction<T> where T : class
        {
            private readonly MethodInfo _method;

            private sealed class WeakActionInstance : WeakAction<T>
            {
                private readonly WeakReference _weakRef;

                private WeakActionInstance(MethodInfo method)
                    : base(method)
                {
                }

                public WeakActionInstance(object instance, MethodInfo method)
                    : this(method)
                {
                    if (instance == null)
                        throw new ArgumentNullException(nameof(instance), "instance must not be null");
                    _weakRef = new WeakReference(instance);
                }

                private bool HasBeenCollected => !_weakRef.IsAlive;

                protected override T GetMethod()
                {
                    if (HasBeenCollected) { return null; }

                    var localTarget = _weakRef.Target;
                    if (localTarget == null) { return null; }

                    return System.Delegate.CreateDelegate(typeof(T), localTarget, _method.Name) as T;
                }
            }

            private WeakAction(MethodInfo method) => _method = method;

            public static WeakAction<T> Create(object instance, Expression expression) => new WeakActionInstance(instance, GetMethodInfo(expression));

            protected abstract T GetMethod();

            public T Delegate => GetMethod();

            private static MethodInfo GetMethodInfo(Expression expression)
            {
                var lambda = expression as LambdaExpression;
                if (lambda == null)
                    throw new ArgumentException("expression is not LambdaExpression");

                var outermostExpression = lambda.Body as MethodCallExpression;

                if (outermostExpression == null)
                    throw new ArgumentException("Invalid Expression. Expression should consist of a Method call only.");

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

        public override Task Handle(IMessage message) => Handle((TMessage) message);
    }
}