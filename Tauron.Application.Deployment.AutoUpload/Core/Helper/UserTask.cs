using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Tauron.Application.Deployment.AutoUpload.Core.Helper
{
    [DebuggerStepThrough]
    public class UserResultTask<TResult> : ITask
    {
        public UserResultTask(Func<TResult> callback, bool sync)
        {
            _callback = callback ?? throw new ArgumentNullException(nameof(callback));
            Synchronize = sync;
            _task = new TaskCompletionSource<TResult>();
        }
        
        private readonly Func<TResult> _callback;

        private readonly TaskCompletionSource<TResult> _task;

        public void ExecuteAsync() => Task.Run(ExecuteSync);

        public void ExecuteSync()
        {
            try
            {
                _task.SetResult(_callback());
            }
            catch (Exception e)
            {
                _task.SetException(e);
            }
        }

        public bool Synchronize { get; }
        
        public Task Task => _task.Task;
        
    }
    
    [DebuggerStepThrough]
    public class UserTask : ITask
    {
        public UserTask(Action callback, bool sync)
        {
            _callback = callback;
            Synchronize = sync;
            _task = new TaskCompletionSource<object>();
        }
        
        private readonly Action _callback;

        private readonly TaskCompletionSource<object> _task;

        public void ExecuteAsync() => Task.Run(ExecuteSync);

        public void ExecuteSync()
        {
            try
            {

                _callback();
                _task.SetResult(null!);
            }
            catch (Exception e)
            {
                _task.SetException(e);
            }
        }

        public bool Synchronize { get; }
        
        public Task Task => _task.Task;
    }
}