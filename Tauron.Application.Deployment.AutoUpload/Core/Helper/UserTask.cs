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

        public Task ExecuteAsync() => Task.Run(ExecuteSync);

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

        public UserTask(Func<Task> asyncCallback, bool sync)
        {
            _asyncCallback = asyncCallback;
            Synchronize = sync;
            _task = new TaskCompletionSource<object>();
        }

        private readonly Action? _callback;
        private readonly Func<Task>? _asyncCallback;

        private readonly TaskCompletionSource<object> _task;

        public async Task ExecuteAsync()
        {

            try
            {
                if (_callback != null)
                    await Task.Run(ExecuteSync);
                if (_asyncCallback != null)
                    await _asyncCallback();
            }
            catch (Exception e)
            {
                _task.SetException(e);
            }
        }

        public void ExecuteSync()
        {
            try
            {
                _callback?.Invoke();
                _asyncCallback?.Invoke().Wait();
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