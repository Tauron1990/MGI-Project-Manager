using System;
using CQRSlite.Commands;
using CQRSlite.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Tauron.CQRS.Services.Core
{
    public sealed class AwaiterFactory : IAwaiterFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public AwaiterFactory(IServiceProvider serviceProvider) 
            => _serviceProvider = serviceProvider;

        public AwaiterBase<TMessage, TRespond> CreateAwaiter<TMessage, TRespond>() where TMessage : class, ICommand where TRespond : IEvent
            => _serviceProvider.GetRequiredService<AwaiterBase<TMessage, TRespond>>();
    }
}