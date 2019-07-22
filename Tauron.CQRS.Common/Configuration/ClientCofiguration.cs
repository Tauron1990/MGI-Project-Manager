using System;
using System.Collections.Generic;
using CQRSlite.Commands;
using CQRSlite.Events;

namespace Tauron.CQRS.Common.Configuration
{
    public class ClientCofiguration : CommonConfiguration
    {
        private readonly Dictionary<string, Type> HandlerRegistry = new Dictionary<string, Type>();
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

        public Dictionary<string, Type> GetHandlers () => new Dictionary<string, Type>(HandlerRegistry);

        public ClientCofiguration RegisterCancellableEventHandler<TEvent, THandler>()
            where TEvent : IEvent 
            where THandler : ICancellableEventHandler<TEvent>

        {
            HandlerRegistry[typeof(TEvent).Name] = typeof(THandler);
            return this;
        }

        public ClientCofiguration RegisterCancellableCommandHandler<TCommand, THandler>()
            where TCommand : ICommand
            where THandler : ICancellableCommandHandler<TCommand>

        {
            HandlerRegistry[typeof(TCommand).Name] = typeof(THandler);
            return this;
        }

        public ClientCofiguration RegisterEventHandler<TEvent, THandler>()
            where TEvent : IEvent
            where THandler : IEventHandler<TEvent>

        {
            HandlerRegistry[typeof(TEvent).Name] = typeof(THandler);
            return this;
        }

        public ClientCofiguration RegisterCommandHandler<TCommand, THandler>()
            where TCommand : ICommand
            where THandler : ICommandHandler<TCommand>

        {
            HandlerRegistry[typeof(TCommand).Name] = typeof(THandler);
            return this;
        }

        internal void RegisterHandler(string name, Type type) 
            => HandlerRegistry[name] = type;
    }
}