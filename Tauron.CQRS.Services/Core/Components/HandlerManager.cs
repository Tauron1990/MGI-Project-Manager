using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Commands;
using CQRSlite.Events;
using CQRSlite.Messages;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tauron.CQRS.Common.Configuration;

namespace Tauron.CQRS.Services.Core.Components
{
    [UsedImplicitly]
    public class HandlerManager : IHandlerManager, IDisposable
    {
        private class HandlerListDelegator
        {
            private readonly List<HandlerInstace> _handlers;

            public HandlerListDelegator(List<HandlerInstace> handlers) => _handlers = handlers;

            public async Task Handle(IMessage msg, CancellationToken token)
            {
                foreach (var handlerInstace in _handlers) await handlerInstace.Handle(msg, token);
            }
        }
        private class HandlerInstace
        {
            private abstract class InvokerBase
            {
                public abstract Task Invoke(IMessage msg, CancellationToken token);
            }
            private abstract class InvokerHelper : InvokerBase
            {
                private readonly Type _targetType;
                private readonly Type _targetInterface;
                private MethodInfo _methodInfo;

                protected InvokerHelper(Type targetType, Type targetInterface)
                {
                    _targetType = targetType;
                    _targetInterface = targetInterface;
                }

                protected MethodInfo GetMethod()
                {
                    if (_methodInfo != null) return _methodInfo;
                    var temp = _targetType.GetInterfaceMap(_targetInterface.GetInterfaces().Single());
                    _methodInfo = temp.InterfaceMethods.Single();
                    return _methodInfo;
                }
            }

            private class Command : InvokerHelper
            {
                private readonly Func<object> _handler;

                public Command(Func<object> handler, Type targetType, Type targetInterface) : base(targetType, targetInterface) => _handler = handler;

                protected override async Task Invoke(IMessage msg, CancellationToken token, Func<ICommand, Task> del)
                    => await (Task)GetMethod().invoke;
            }
            private class CancelCommand : InvokerHelper<Func<ICommand, CancellationToken, Task>>
            {
                private readonly Func<object> _handler;

                public CancelCommand(Func<object> handler, Type targetType, Type targetInterface) : base(targetType, targetInterface) => _handler = handler;

                protected override Func<ICommand, CancellationToken, Task> Create()
                    => (Func<ICommand, CancellationToken, Task>) Delegate.CreateDelegate(typeof(Func<ICommand, CancellationToken, Task>),
                        _handler(), GetMethod());

                protected override async Task Invoke(IMessage msg, CancellationToken token, Func<ICommand, CancellationToken, Task> del)
                    => await del((ICommand)msg, token);
            }
            private class Event : InvokerHelper<Func<IEvent, Task>>
            {
                private readonly Func<object> _handler;

                public Event(Func<object> handler, Type targetType, Type targetInterface) : base(targetType, targetInterface) => _handler = handler;

                protected override Func<IEvent, Task> Create()
                    => (Func<IEvent, Task>)Delegate.CreateDelegate(typeof(Func<IEvent, Task>), _handler(), GetMethod());

                protected override async Task Invoke(IMessage msg, CancellationToken token, Func<IEvent, Task> del)
                    => await del((IEvent)msg);
            }
            private class CancelEvent : InvokerHelper<Func<IEvent, CancellationToken, Task>>
            {
                private readonly Func<object> _handler;

                public CancelEvent(Func<object> handler, Type targetType, Type targetInterface) : base(targetType, targetInterface) => _handler = handler;
                
                protected override Func<IEvent, CancellationToken, Task> Create()
                    => (Func<IEvent, CancellationToken, Task>)Delegate.CreateDelegate(typeof(Func<IEvent, CancellationToken, Task>),
                        _handler(), GetMethod());

                protected override async Task Invoke(IMessage msg, CancellationToken token, Func<IEvent, CancellationToken, Task> del)
                    => await del((IEvent)msg, token);
            }

            private readonly Dictionary<Type, InvokerBase> _invoker = new Dictionary<Type, InvokerBase>();
            
            public bool IsCommand { get; }

            public HandlerInstace(Func<object> target, Type handlerType)
            {
                var inters = handlerType.GetInterfaces();

                foreach (var i in inters.Where(i => i.IsGenericType))
                {
                    var targetType = i.GetGenericTypeDefinition();

                    if(!IsCommand)
                        IsCommand = targetType == typeof(ICommandHandler<>) || targetType == typeof(ICancellableCommandHandler<>);
                    Type key = i.GetGenericArguments()[0];


                    if (targetType == typeof(ICommandHandler<>)) _invoker[key] = new Command(target, handlerType, i);
                    else if (targetType == typeof(ICancellableCommandHandler<>)) _invoker[key] = new CancelCommand(target, handlerType, i);
                    else if (targetType == typeof(IEventHandler<>)) _invoker[key] = new Event(target, handlerType, i);
                    else if (targetType == typeof(ICancellableEventHandler<>)) _invoker[key] = new CancelEvent(target, handlerType, i);
                }

                if(_invoker.Count == 0)
                    throw new InvalidOperationException("No Invoker Found!");
            }

            public async Task Handle(IMessage msg, CancellationToken token) => await _invoker[msg.GetType()].Invoke(msg, token);
        }

        private readonly IOptions<ClientCofiguration> _configuration;
        private readonly IDispatcherClient _client;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        // ReSharper disable once CollectionNeverQueried.Local
        private readonly List<HandlerListDelegator> _handlerInstaces = new List<HandlerListDelegator>();

        public HandlerManager(IOptions<ClientCofiguration> configuration, IDispatcherClient client, IServiceScopeFactory serviceScopeFactory)
        {
            _configuration = configuration;
            _client = client;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Init(CancellationToken token)
        {
            await _client.Start(token);

            foreach (var handler in _configuration.Value.GetHandlers())
            {
                var commands = new List<HandlerInstace>();
                var events = new List<HandlerInstace>();

                foreach (var handlerInstace in handler.Value
                                .Select(h => new HandlerInstace(() =>
                                                                {
                                                                    using var scope = _serviceScopeFactory.CreateScope();
                                                                    return ActivatorUtilities.GetServiceOrCreateInstance(scope.ServiceProvider, h);
                                                                }, h)))
                { 
                    if(handlerInstace.IsCommand)
                        commands.Add(handlerInstace);
                    else
                        events.Add(handlerInstace);
                }


                if (commands.Count != 0)
                {
                    var del = new HandlerListDelegator(commands);
                    await _client.Subsribe(handler.Key, del.Handle, true);
                    _handlerInstaces.Add(del);
                }
                else
                {
                    var del = new HandlerListDelegator(events);
                    await _client.Subsribe(handler.Key, del.Handle, false);
                    _handlerInstaces.Add(del);
                }
            }
        }

        public void Dispose() 
            => _handlerInstaces.Clear();
    }
}