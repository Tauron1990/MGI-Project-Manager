using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Tauron.CQRS.Common.Configuration;
using Tauron.CQRS.Common.Dto;
using Tauron.CQRS.Common.ServerHubs;
using Tauron.CQRS.Server.EventStore;
using Tauron.CQRS.Server.Hubs;

namespace Tauron.CQRS.Server
{
    [UsedImplicitly]
    public class DispatcherService : BackgroundService
    {
        private readonly IEventManager _eventManager;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IOptions<ServerConfiguration> _config;
        private readonly ILogger<DispatcherService> _logger;

        private bool _stopped;
        //private IServiceScope _serviceScope;
        //private DispatcherDatabaseContext _dispatcherDatabaseContext;
        private Task _runningTask;

        public DispatcherService(IEventManager eventManager, ILogger<DispatcherService> logger, IServiceScopeFactory scopeFactory, IOptions<ServerConfiguration> config)
        {
            _eventManager = eventManager;
            _scopeFactory = scopeFactory;
            _config = config;
            _logger = logger;
        }

        #region Overrides of BackgroundService

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_config.Value.Memory)
            {
                using var prov = _scopeFactory.CreateScope();
                using var context = prov.ServiceProvider.GetRequiredService<DispatcherDatabaseContext>();
                context.Database.Migrate();
            }

            _eventManager.Dispatcher.OnWork += async domainEvent =>
            {
                if(stoppingToken.IsCancellationRequested) _eventManager.Dispatcher.Stop();

                try
                {
                    switch (domainEvent.RealMessage.EventName)
                    {
                        case HubEventNames.DispatcherCommand.StartDispatcher:
                            _logger.LogInformation("Starting Dispatcher");
                            _stopped = false;
                            await _eventManager.StartDispatching();
                            break;
                        case HubEventNames.DispatcherCommand.StopDispatcher:
                            _logger.LogInformation("Stopping Dispatcher");
                            _stopped = true;
                            await _eventManager.StopDispatching();
                            break;
                        default:
                            if (_stopped)
                                return;
                            //if (_serviceScope == null || _dispatcherDatabaseContext == null)
                            //{
                            //    _serviceScope = _scopeFactory.CreateScope();
                            //    _dispatcherDatabaseContext = _serviceScope.ServiceProvider
                            //        .GetRequiredService<DispatcherDatabaseContext>();
                            //}

                            if (domainEvent.RealMessage.EventType == EventType.Event)
                            {
                                //await _dispatcherDatabaseContext.SaveChangesAsync(stoppingToken);
                                if (stoppingToken.IsCancellationRequested) return;

                                if (!await _eventManager.DeliverEvent(domainEvent, stoppingToken))
                                    await _eventManager.DeliverEvent(CreateEventFailedEvent(domainEvent), stoppingToken);
                            }
                            else
                            {
                                if (!await _eventManager.DeliverEvent(domainEvent, stoppingToken))
                                    await _eventManager.DeliverEvent(CreateEventFailedEvent(domainEvent), stoppingToken);
                            }

                            //if (_eventManager.Dispatcher.Count == 0 || stoppingToken.IsCancellationRequested)
                            //{
                            //    _dispatcherDatabaseContext?.Dispose();
                            //    _serviceScope?.Dispose();

                            //    _dispatcherDatabaseContext = null;
                            //    _serviceScope = null;
                            //}

                            break;
                    }

                }
                catch (OperationCanceledException)
                {
                }
            };

            _runningTask = _eventManager.Dispatcher.Start();

            return Task.CompletedTask;
        }


        private static RecivedDomainEvent CreateEventFailedEvent(RecivedDomainEvent original)
        {
            return new RecivedDomainEvent(new ServerDomainMessage
            {
                EventData = JsonConvert.SerializeObject(new EventFailedEventMessage{ EventName = original.RealMessage.EventName }),
                EventName = HubEventNames.DispatcherEvent.DeliveryFailedEvent,
                TypeName = typeof(EventFailedEventMessage).AssemblyQualifiedName,
                EventType = EventType.Event, SequenceNumber = -1
            }, string.Empty);
        }

        #endregion

        public override void Dispose()
        {
            base.Dispose();
            _runningTask.Dispose();
        }
    }
}