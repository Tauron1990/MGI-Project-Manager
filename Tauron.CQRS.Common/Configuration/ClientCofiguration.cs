using System;
using System.Collections.Generic;
using CQRSlite.Commands;
using CQRSlite.Events;
using JetBrains.Annotations;

namespace Tauron.CQRS.Common.Configuration
{
    [PublicAPI]
    public class ClientCofiguration : CommonConfiguration
    {
        private readonly Dictionary<string, HashSet<Type>> _handlerRegistry = new Dictionary<string, HashSet<Type>>();
        private string _serviceName;

        public string EventHubUrl { get; set; }

        public string EventServerApiUrl { get; set; }

        public string PersistenceApiUrl { get; set; }

        public string ApiKey { get; set; }

        public string ServiceName
        {
            get
            {
                if(string.IsNullOrWhiteSpace(_serviceName))
                    throw new InvalidOperationException("Need Servicename for Operation");
                return _serviceName;
            }
            set => _serviceName = value;
        }

        public Dictionary<string, HashSet<Type>> GetHandlers () => new Dictionary<string, HashSet<Type>>(_handlerRegistry);

        private void AddHandler(string name, Type type)
        {
            if (_handlerRegistry.TryGetValue(name, out var list)) list.Add(type);
            else _handlerRegistry.Add(name, new HashSet<Type> {type});
        }

        public bool IsHandlerRegistrated<TCommand, THandler>() =>
            _handlerRegistry.TryGetValue(typeof(TCommand).Name, out var list) && list.Contains(typeof(THandler));

        public ClientCofiguration RegisterCancellableEventHandler<TEvent, THandler>()
            where TEvent : IEvent 
            where THandler : ICancellableEventHandler<TEvent>

        {
            AddHandler(typeof(TEvent).Name, typeof(THandler));
            return this;
        }

        public ClientCofiguration RegisterCancellableCommandHandler<TCommand, THandler>()
            where TCommand : ICommand
            where THandler : ICancellableCommandHandler<TCommand>

        {
            AddHandler(typeof(TCommand).Name, typeof(THandler));
            return this;
        }

        public ClientCofiguration RegisterEventHandler<TEvent, THandler>()
            where TEvent : IEvent
            where THandler : IEventHandler<TEvent>

        {
            AddHandler(typeof(TEvent).Name, typeof(THandler));
            return this;
        }

        public ClientCofiguration RegisterCommandHandler<TCommand, THandler>()
            where TCommand : ICommand
            where THandler : ICommandHandler<TCommand>

        {
            AddHandler(typeof(TCommand).Name, typeof(THandler));
            return this;
        }

        internal void RegisterHandler(string name, Type type)
            => AddHandler(name, type);
    }
}