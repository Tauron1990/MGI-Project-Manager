using System;
using System.Collections.Generic;
using System.Linq;
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
        private class HandlerInstace
        {
            private abstract class InvokerBase
            {
                public abstract Task Invoke(IMessage msg, CancellationToken token);
            }
            private abstract class InvokerHelper<TDelegate> : InvokerBase
            {
                private TDelegate _delegate;

                public override async Task Invoke(IMessage msg, CancellationToken token)
                {
                    if (_delegate == null)
                        _delegate = Create();

                    await Invoke(msg, token, _delegate);
                }

                protected abstract TDelegate Create();
                protected abstract Task Invoke(IMessage msg, CancellationToken token, TDelegate del);
            }

            private class Command : InvokerHelper<Func<ICommand, Task>>
            {
                private readonly object _handler;

                public Command(object handler) => _handler = handler;

                protected override Func<ICommand, Task> Create() 
                    => (Func<ICommand, Task>)Delegate.CreateDelegate(typeof(Func<ICommand, Task>), _handler, _handler.GetType().GetMethod("Handle") ?? throw new InvalidOperationException());

                protected override async Task Invoke(IMessage msg, CancellationToken token, Func<ICommand, Task> del)
                    => await del((ICommand) msg);
            }
            private class CancelCommand : InvokerHelper<Func<ICommand, CancellationToken, Task>>
            {
                private readonly object _handler;

                public CancelCommand(object handler) => _handler = handler;

                protected override Func<ICommand, CancellationToken, Task> Create()
                    => (Func<ICommand, CancellationToken, Task>) Delegate.CreateDelegate(typeof(Func<ICommand, CancellationToken, Task>),
                        _handler, _handler.GetType().GetMethod("Handle") ?? throw new InvalidOperationException());

                protected override async Task Invoke(IMessage msg, CancellationToken token, Func<ICommand, CancellationToken, Task> del)
                    => await del((ICommand)msg, token);
            }
            private class Event : InvokerHelper<Func<IEvent, Task>>
            {
                private readonly object _handler;

                public Event(object handler) => _handler = handler;

                protected override Func<IEvent, Task> Create()
                    => (Func<IEvent, Task>)Delegate.CreateDelegate(typeof(Func<IEvent, Task>), _handler, _handler.GetType().GetMethod("Handle") ?? throw new InvalidOperationException());

                protected override async Task Invoke(IMessage msg, CancellationToken token, Func<IEvent, Task> del)
                    => await del((IEvent)msg);
            }
            private class CancelEvent : InvokerHelper<Func<IEvent, CancellationToken, Task>>
            {
                private readonly object _handler;

                public CancelEvent(object handler) => _handler = handler;
                
                protected override Func<IEvent, CancellationToken, Task> Create()
                    => (Func<IEvent, CancellationToken, Task>)Delegate.CreateDelegate(typeof(Func<IEvent, CancellationToken, Task>),
                        _handler, _handler.GetType().GetMethod("Handle") ?? throw new InvalidOperationException());

                protected override async Task Invoke(IMessage msg, CancellationToken token, Func<IEvent, CancellationToken, Task> del)
                    => await del((IEvent)msg, token);
            }

            private readonly Dictionary<Type, InvokerBase> _invoker = new Dictionary<Type, InvokerBase>();
            
            public bool IsCommand { get; }

            public HandlerInstace(object target)
            {
                var inters = target.GetType().GetInterfaces();

                foreach (var i in inters.Where(i => i.IsGenericType))
                {
                    var targetType = i.GetGenericTypeDefinition();

                    IsCommand = targetType == typeof(ICommandHandler<>) || targetType == typeof(ICancellableCommandHandler<>);
                    Type key = i.GetGenericArguments()[0];


                    if (targetType == typeof(ICommandHandler<>)) _invoker[key] = new Command(target);
                    else if (targetType == typeof(ICancellableCommandHandler<>)) _invoker[key] = new CancelCommand(target);
                    else if (targetType == typeof(IEventHandler<>)) _invoker[key] = new Event(target);
                    else if (targetType == typeof(ICancellableEventHandler<>)) _invoker[key] = new CancelEvent(target);
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
        private readonly List<HandlerInstace> _handlerInstaces = new List<HandlerInstace>();

        public HandlerManager(IOptions<ClientCofiguration> configuration, IDispatcherClient client, IServiceScopeFactory serviceScopeFactory)
        {
            _configuration = configuration;
            _client = client;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Init(CancellationToken token)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            foreach (var handler in _configuration.Value.GetHandlers())
            {
                var handlerObject = ActivatorUtilities.GetServiceOrCreateInstance(scope.ServiceProvider, handler.Value);
                var inst = new HandlerInstace(handlerObject);

                await _client.Subsribe(handler.Key, inst.Handle, inst.IsCommand);
                _handlerInstaces.Add(inst);
            }
            
            await _client.Start(token);
        }

        public void Dispose() 
            => _handlerInstaces.Clear();
    }
}