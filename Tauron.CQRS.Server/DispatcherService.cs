using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tauron.CQRS.Server.Core;
using Tauron.CQRS.Server.EventStore;
using Tauron.CQRS.Server.EventStore.Data;

namespace Tauron.CQRS.Server
{
    [UsedImplicitly]
    public class DispatcherService : BackgroundService
    {
        private readonly IEventManager _eventManager;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IApiKeyStore _apiKeyStore;

        private IServiceScope _serviceScope;
        private DispatcherDatabaseContext _dispatcherDatabaseContext;

        public DispatcherService(IEventManager eventManager, IServiceScopeFactory scopeFactory, IApiKeyStore apiKeyStore)
        {
            _eventManager = eventManager;
            _scopeFactory = scopeFactory;
            _apiKeyStore = apiKeyStore;
        }

        #region Overrides of BackgroundService

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach (var domainEvent in _eventManager.Dispatcher.GetConsumingEnumerable(stoppingToken))
            {
                if (_serviceScope == null || _dispatcherDatabaseContext == null)
                {
                    _serviceScope = _scopeFactory.CreateScope();
                    _dispatcherDatabaseContext = _serviceScope.ServiceProvider.GetRequiredService<DispatcherDatabaseContext>();
                }

                var entity = _dispatcherDatabaseContext.EventEntities.Add(new EventEntity
                {
                    Origin = await _apiKeyStore.GetServiceFromKey(domainEvent.ApiKey),
                    Data = domainEvent.DomainEvent.EventData,
                    EventName = domainEvent.DomainEvent.EventName,
                    EventType = domainEvent.DomainEvent.EventType,
                    EventStatus = EventStatus.Pending
                });

                await _dispatcherDatabaseContext.SaveChangesAsync(stoppingToken);
                if(stoppingToken.IsCancellationRequested) continue;
                domainEvent.DomainEvent.SequenceNumber = entity.Entity.SequenceNumber;

                if (await _eventManager.DeliverEvent(domainEvent))
                {
                    entity.Entity.EventStatus = EventStatus.Deliverd;
                    await _dispatcherDatabaseContext.SaveChangesAsync(stoppingToken);
                }
                else
                {
                    entity.Entity.EventStatus = EventStatus.Failed;
                    await _dispatcherDatabaseContext.SaveChangesAsync(stoppingToken);
                }

                if (_eventManager.Dispatcher.Count == 0 || stoppingToken.IsCancellationRequested)
                {
                    _dispatcherDatabaseContext?.Dispose();
                    _serviceScope?.Dispose();

                    _dispatcherDatabaseContext = null;
                    _serviceScope = null;
                }
            }
        }

        #endregion
    }
}