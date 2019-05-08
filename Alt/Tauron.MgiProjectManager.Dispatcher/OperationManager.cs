using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tauron.MgiProjectManager.Data;
using Tauron.MgiProjectManager.Data.Models;
using Tauron.MgiProjectManager.Data.Repositorys;
using Tauron.MgiProjectManager.Dispatcher.Actions;
using Tauron.MgiProjectManager.Dispatcher.Model;

namespace Tauron.MgiProjectManager.Dispatcher
{
    [Export(typeof(IOperationManager))]
    public class OperationManager : IOperationManager
    {
        private readonly IOperationRepository _operationRepository;
        private readonly ILogger<OperationManager> _logger;
        private readonly IBackgroundTaskDispatcher _dispatcher;
        private readonly IServiceProvider _serviceProvider;

        public OperationManager(IUnitOfWork unitOfWork, ILogger<OperationManager> logger, IBackgroundTaskDispatcher dispatcher, IServiceProvider serviceProvider)
        {
            _operationRepository = unitOfWork.OperationRepository;
            _logger = logger;
            _dispatcher = dispatcher;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> AddOperation(OperationSetup op)
        {
            var ent = op.ToOperation();
            await _operationRepository.AddOperation(ent.ToOperationEntity());

            return ent.OperationId;
        }

        public async Task UpdateOperation(string id, Action<IDictionary<string, string>> toUpdate)
        {
            await _operationRepository.UpdateOperation(id, entity =>
            {
                var tempContext = entity.Context.ToList();
                List<OperationContextEntity> contextEntities = new List<OperationContextEntity>();

                var context = tempContext.ToDictionary(e => e.Name, e => e.Value);
                toUpdate(context);

                foreach (var contextKey in context)
                {
                    var ent = tempContext.FirstOrDefault(e => e.Name == contextKey.Key);
                    if (ent != null)
                    {
                        tempContext.Remove(ent);
                        ent.Value = contextKey.Value;
                        contextEntities.Add(ent);
                    }
                    else
                    {
                        contextEntities.Add(new OperationContextEntity
                        {
                            Name = contextKey.Key,
                            Value = contextKey.Value
                        });
                    }
                }

                entity.Context.Clear();
                entity.Context.AddRange(contextEntities);
            });
        }

        public async Task<IReadOnlyDictionary<string, string>> SearchOperation(string id)
        {
            var ent = await _operationRepository.Find(id);
            var dic = ent.Context.ToDictionary(e => e.Name, e => e.Value);

            dic.Add(OperationMeta.OperationType, ent.OperationType);
            dic.Add(OperationMeta.OperationName, ent.CurrentOperation);

            return dic;
        }

        public async Task ExecuteNext(string id)
        {
            var opE = await _operationRepository.Find(id);
            var op = opE.ToOperation();

            await _dispatcher.SheduleTask(async (prov) =>
            {
                var action = prov.GetRequiredService<IEnumerable<IOperationAction>>().First(a => a.Name == op.CurrentOperation);

                try
                {
                    var operations = await action.Execute(op);
                    await _operationRepository.CompledOperation(op.OperationId);
                    await action.PostExecute(op);

                    await Task.WhenAll(operations.Select(AddOperation));
                }
                catch (Exception e)
                {
                    try
                    {
                        _logger.LogError(e, $"Error Running Action: {action.Name}");
                        await action.Error(op, e);
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, $"Error Error notify Action: {action.Name}");
                    }
                }
            });
        }

        public async Task RemoveAction(string id)
        {
            var ent = await _operationRepository.Find(id);
            var op = ent.ToOperation();

            var action = await GetActions().ToAsyncEnumerable().First(e => e.Name == ent.CurrentOperation);
            var result = await action.Remove(op);

            if (result)
                await _operationRepository.Remove(op.OperationId);
        }

        public async Task CleanUpExpiryOperation()
        {
            var currentDate = DateTime.Now;

            var enumerable = await _operationRepository.GetAllOperations();
            foreach (var operation in enumerable)
            {
                if (operation.ExpiryDate < currentDate)
                    await RemoveAction(operation.OperationId);
            }
        }

        public async Task<string[]> GetOperations(Predicate<OperationFilter> filter)
        {
            return (await _operationRepository.GetAllOperations())
                .Select(oe => OperationFilter.FromOperation(oe.ToOperation()))
                .Where(op => filter?.Invoke(op) ?? true)
                .Select(of => of.OperationId)
                .ToArray();
        }

        public async Task<IDictionary<string, string>> GetContext(string id) 
            => (await _operationRepository.Find(id)).ToOperation().OperationContext;

        private IEnumerable<IOperationAction> GetActions() 
            => _serviceProvider.GetRequiredService<IEnumerable<IOperationAction>>();
    }
}