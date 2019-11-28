using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Catel.Logging;
using Catel.Services;
using JetBrains.Annotations;

namespace Tauron.Application.Deployment.AutoUpload.Core.Helper
{
    public interface ITaskScheduler : IDisposable
    {
        Task QueueTask([Catel.Fody.NotNull] ITask task);
    }
    
    [PublicAPI, DebuggerStepThrough]
    public sealed class InternalTaskScheduler : ITaskScheduler
    {
        private readonly string _name;

        public bool Disposed { get; private set; }

        private readonly BlockingCollection<ITask> _collection;
        
        private readonly IDispatcherService _synchronizationContext;
        private bool _predisposed;

        private Task _task;
        

        public InternalTaskScheduler(IDispatcherService synchronizationContext, string name)
        {
            _synchronizationContext = synchronizationContext;
            _name = name;
            _collection = new BlockingCollection<ITask>();
        }

        ~InternalTaskScheduler() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        public Task QueueTask(ITask task)
        {
            CheckDispose();
            if (task.Synchronize && _synchronizationContext != null)
                return _synchronizationContext.InvokeAsync(task.Execute);

            if (_collection == null)
            {
                task.Execute();
                var tcs = new TaskCompletionSource<object>();
                tcs.SetResult(null!);
                return tcs.Task;
            }

            if (_collection.IsAddingCompleted) return Task.CompletedTask;
            _collection.Add(task);
            return task.Task;
        }

        internal void EnterLoop()
        {
            var source = new TaskCompletionSource<object>();
            _task = source.Task;
            EnterLoopPrivate();
            source.SetResult(null!);
        }

        private void EnterLoopPrivate()
        {
            var thread = Thread.CurrentThread;
            if (string.IsNullOrWhiteSpace(thread.Name) && !string.IsNullOrWhiteSpace(_name))
                thread.Name = _name;

            foreach (var task in _collection.GetConsumingEnumerable())
                try
                {
                    task.Execute();
                }
                catch (Exception e)
                {
                    LogManager.GetLogger("TauronTaskScheduler").Error(e);
                    throw;
                }

            _collection.Dispose();
            Disposed = true;
        }

        public void Start() => _task = Task.Factory.StartNew(EnterLoopPrivate, TaskCreationOptions.LongRunning);

        private void CheckDispose()
        {
            if (Disposed)
                throw new ObjectDisposedException("TaskScheduler");
        }

        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_collection")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "disposing")]
        // ReSharper disable UnusedParameter.Local
        private void Dispose(bool disposing)
        {
            // ReSharper restore UnusedParameter.Local
            if (_predisposed) return;

            _predisposed = true;

            _collection?.CompleteAdding();
            _task?.Wait();
        }
    }
}