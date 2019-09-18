using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tauron.CQRS.Common.Dto;
using Tauron.CQRS.Common.ServerHubs;
using Tauron.CQRS.Server.Hubs;

namespace Tauron.CQRS.Server
{
    [UsedImplicitly]
    public class DispatcherService : BackgroundService
    {
        private readonly IEventManager _eventManager;
        //private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DispatcherService> _logger;

        private bool _stopped;
        //private IServiceScope _serviceScope;
        //private DispatcherDatabaseContext _dispatcherDatabaseContext;
        private Task _runningTask;

        public DispatcherService(IEventManager eventManager, ILogger<DispatcherService> logger)
        {
            _eventManager = eventManager;
            //_scopeFactory = scopeFactory;
            _logger = logger;
        }

        #region Overrides of BackgroundService

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _runningTask = Task.Run(async () =>
            {
                try
                {
                    foreach (var domainEvent in _eventManager.Dispatcher.GetConsumingEnumerable(stoppingToken))
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
                                    continue;
                                //if (_serviceScope == null || _dispatcherDatabaseContext == null)
                                //{
                                //    _serviceScope = _scopeFactory.CreateScope();
                                //    _dispatcherDatabaseContext = _serviceScope.ServiceProvider
                                //        .GetRequiredService<DispatcherDatabaseContext>();
                                //}

                                if (domainEvent.RealMessage.EventType == EventType.TransistentEvent)
                                {
                                    //await _dispatcherDatabaseContext.SaveChangesAsync(stoppingToken);
                                    if (stoppingToken.IsCancellationRequested) continue;

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
                }
                catch (OperationCanceledException)
                {
                }
            }, stoppingToken);

            return Task.CompletedTask;
        }


        private static RecivedDomainEvent CreateEventFailedEvent(RecivedDomainEvent original)
        {
            return new RecivedDomainEvent(new ServerDomainMessage
            {
                EventData = JsonConvert.SerializeObject(new EventFailedEventMessage{ EventName = original.RealMessage.EventName }),
                EventName = HubEventNames.DispatcherEvent.DeliveryFailedEvent,
                TypeName = typeof(EventFailedEventMessage).AssemblyQualifiedName,
                EventType = EventType.TransistentEvent, SequenceNumber = -1
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