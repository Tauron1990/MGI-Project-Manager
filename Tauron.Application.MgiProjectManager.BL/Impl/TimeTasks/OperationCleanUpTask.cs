using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Tauron.Application.MgiProjectManager.BL.Contracts;

namespace Tauron.Application.MgiProjectManager.BL.Impl.TimeTasks
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public sealed class OperationCleanUpTask : ITimeTask
    {
        private readonly IOperationManager _operationManager;

        public OperationCleanUpTask(IOperationManager operationManager) 
            => _operationManager = operationManager;

        public string Name => nameof(OperationCleanUpTask);

        public TimeSpan Interval => TimeSpan.FromDays(1);

        public async Task TriggerAsync() 
            => await _operationManager.CleanUpExpiryOperation();
    }
}