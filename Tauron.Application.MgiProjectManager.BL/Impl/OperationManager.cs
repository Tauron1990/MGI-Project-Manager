using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tauron.Application.MgiProjectManager.BL.Contracts;
using Tauron.Application.MgiProjectManager.Server.Data.Entitys;
using Tauron.Application.MgiProjectManager.Server.Data.Repository;

namespace Tauron.Application.MgiProjectManager.BL.Impl
{
    public class OperationManager : IOperationManager
    {
        private readonly IEnumerable<IOperationAction> _operationActions;
        private readonly IOperationRepository _operationRepository;
        private readonly ILogger<OperationManager> _logger;

        private ConcurrentBag<Operation> _operations;

        public IEnumerable<Operation> Operations => _operations;

        public OperationManager(IEnumerable<IOperationAction> operationActions, IOperationRepository operationRepository, ILogger<OperationManager> logger)
        {
            _operationActions = operationActions;
            _operationRepository = operationRepository;
            _logger = logger;

            _operationRepository
                .GetAllOperations()
                .ContinueWith(
                    t => _operations = new ConcurrentBag<Operation>(
                        t.Result.Select(e => new Operation(e.OperationId, 
                            e.NextOperation, 
                            e.OperationType,
                            e.Context.ToDictionary(k => k.Name, oe => oe.Value),
                            e.ExpiryDate))));
        }

        public async Task AddOperation(Operation op)
        {
            var ent = CreateEntity(op);
            await _operationRepository.AddOperation(ent);

            _operations.Add(op);
        }

        public Task<Operation> SearchOperation(string id) 
            => Task.FromResult(_operations.First(op => op.OperationId == id));

        public async Task<IEnumerable<Operation>> ExecuteNext(Operation op)
        {
            var action = _operationActions.First(a => a.Name == op.CurrentOperation);

            try
            {
                var operations = await action.Execute(op);
                await _operationRepository.CompledOperation(op.OperationId);
                await Task.WhenAll(operations.Select(AddOperation));

                return operations;
            }
            catch(Exception e)
            {
                _logger.LogError(e, $"Error Runing Action: {action.Name}");
                await action.Error(op, e);

                return new Operation[0];
            }
        }

        public async Task RemoveAction(Operation op)
        {
            var action = _operationActions.First(a => a.Name == op.CurrentOperation);
            var result = await action.Remove(op);

            if (result)
                await _operationRepository.Remove(op.OperationId);
        }

        private OperationEntity CreateEntity(Operation op)
        {
            return new OperationEntity
            {
                ExpiryDate = op.ExpiryDate,
                OperationId = op.OperationId,
                CurrentOperation = op.CurrentOperation,
                OperationType = op.OperationType,
                Context = new List<OperationContextEntity>(op.OperationContext.Select(p => new OperationContextEntity
                {
                    Name = p.Key,
                    Value = p.Value
                }))
            };
        }
    }
}