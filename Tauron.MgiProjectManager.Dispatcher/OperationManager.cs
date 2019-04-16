using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IEnumerable<IOperationAction> _operationActions;
        private readonly IOperationRepository _operationRepository;
        private readonly ILogger<OperationManager> _logger;


        public OperationManager(IEnumerable<IOperationAction> operationActions, IUnitOfWork unitOfWork, ILogger<OperationManager> logger)
        {
            _operationActions = operationActions;
            _operationRepository = unitOfWork.OperationRepository;
            _logger = logger;
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

        public async Task<IEnumerable<string>> ExecuteNext(string id)
        {
            var opE = await _operationRepository.Find(id);
            var op = opE.ToOperation();

            var action = _operationActions.First(a => a.Name == op.CurrentOperation);

            try
            {
                var operations = await action.Execute(op);
                await _operationRepository.CompledOperation(op.OperationId);
                await action.PostExecute(op);

                return await Task.WhenAll(operations.Select(AddOperation));
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error Running Action: {action.Name}");
                await action.Error(op, e);

                return new string[0];
            }
        }

        public async Task RemoveAction(string id)
        {
            var ent = await _operationRepository.Find(id);
            var op = ent.ToOperation();

            var action = await _operationActions.ToAsyncEnumerable().First(e => e.Name == ent.CurrentOperation);
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
    }
}