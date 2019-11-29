using System.Diagnostics;
using System.Threading.Tasks;
using Catel.Services;
using JetBrains.Annotations;

namespace Tauron.Application.Wpf.Helper
{
    public interface ITaskScheduler
    {
        Task QueueTask(ITask task);
    }
    
    [PublicAPI, DebuggerStepThrough]
    public sealed class InternalTaskScheduler : ITaskScheduler
    {
        private readonly IDispatcherService _synchronizationContext;
        
        public InternalTaskScheduler(IDispatcherService synchronizationContext)
        {
            _synchronizationContext = synchronizationContext;
        }

        public Task QueueTask(ITask task)
        {
            if (task.Synchronize && _synchronizationContext != null)
                return _synchronizationContext.InvokeAsync(task.ExecuteSync);


            task.ExecuteAsync();
            return Task.Run(async () => await Task.WhenAll(task.ExecuteAsync(), task.Task));
        }

    }
}