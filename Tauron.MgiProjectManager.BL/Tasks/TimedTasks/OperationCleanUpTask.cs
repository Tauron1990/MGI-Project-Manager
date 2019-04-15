using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Tauron.MgiProjectManager.Dispatcher;
using Tauron.MgiProjectManager.Dispatcher.Actions;

namespace Tauron.MgiProjectManager.BL.Tasks.TimedTasks
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public sealed class OperationCleanUpTask : ITimeTask
    {
        public string Name => nameof(OperationCleanUpTask);

        public TimeSpan Interval => TimeSpan.FromDays(1);

        public async Task TriggerAsync(IServiceProvider serviceProvider) 
            => await serviceProvider.GetRequiredService<IOperationManager>().CleanUpExpiryOperation();
    }
}