using System;
using System.Threading;
using CQRSlite.Events;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceManager.CQRS.Logging;

namespace Tauron.ServiceBootstrapper.Core
{
    public class SendingProvider : ILoggerProvider
    {
        private class Logger : ILogger
        {
            private class ActionDispose : IDisposable
            {
                private readonly Action _action;

                public ActionDispose(Action action) => _action = action;

                public void Dispose() => _action();
            }

            private readonly string _category;
            private readonly Func<IEventPublisher> _factory;
            private int _scope;

            public Logger(string category, Func<IEventPublisher> factory)
            {
                _category = category;
                _factory = factory;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) 
                => _factory()?.Publish(new LoggingEvent(_category, logLevel, eventId, exception, formatter(state, exception), _scope));

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                Interlocked.Increment(ref _scope);
                return new ActionDispose(() => Interlocked.Decrement(ref _scope));
            }
        }

        private Func<IServiceScopeFactory> _factory;

        private IEventPublisher _eventPublisher;

        public SendingProvider([NotNull]Func<IServiceScopeFactory> factory) => _factory = factory;

        [CanBeNull]
        private IEventPublisher GetEventPublisher()
        {
            lock (this)
            {
                if (_eventPublisher != null) return null;

                var temp = _factory();
                if (temp == null) return null;

                using var scope = temp.CreateScope();
                _eventPublisher = scope.ServiceProvider.GetService<IEventPublisher>();
                if (_eventPublisher != null)
                    _factory = null;

                return _eventPublisher;
            }
        }

        public void Dispose()
        {
            _eventPublisher = null;
            _factory = null;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new Logger(categoryName, GetEventPublisher);
        }
    }
}