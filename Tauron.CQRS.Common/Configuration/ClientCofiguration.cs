using System;
using System.Collections.Generic;
using CQRSlite.Commands;
using CQRSlite.Events;

namespace Tauron.CQRS.Common.Configuration
{
    public class ClientCofiguration : CommonConfiguration
    {
        internal readonly Dictionary<string, Type> HandlerRegistry = new Dictionary<string, Type>();

        public string EventHubUrl { get; set; }

        public string EventServerApiUrl { get; set; }

        public string PersistenceApiUrl { get; set; }

        public string ApiKey { get; set; }

        public ClientCofiguration RegisterEventHandler<TEvent, THandler>()
            where TEvent : IEvent 
            where THandler : ICancellableEventHandler<TEvent>

        {
            HandlerRegistry[typeof(TEvent).Name] = typeof(THandler);
            return this;
        }

        public ClientCofiguration RegisterCommandHandler<TCommand, THandler>()
            where TCommand : ICommand
            where THandler : ICancellableCommandHandler<TCommand>

        {
            HandlerRegistry[typeof(TCommand).Name] = typeof(THandler);
            return this;
        }
    }
}