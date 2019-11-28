using System;
using System.Threading.Tasks;
using System.Windows;
using JetBrains.Annotations;
using Tauron.Application.Deployment.AutoUpload.Core.Helper;

namespace Tauron.Application.Deployment.AutoUpload.Core
{
    internal abstract class PipelineBase : IPipeLine, ITask
    {
        protected PipelineBase([NotNull] DependencyObject target, bool simpleMode)
        {
            Source = new WeakReference<DependencyObject>(Argument.NotNull(target, nameof(target)));
            SimpleMode = simpleMode;
            _task = new TaskCompletionSource<object>();
            _task.SetResult(null!);
        }

        private bool SimpleMode
        {
            get => _simpleMode;
            set
            {
                _simpleMode = value;
                var target = Target;

                if (SimpleMode)
                {
                    DataContextServices.UnregisterHandler(target, this);
                    var cont = new FrameworkObject(target, false).DataContext;
                    DataContext = cont == null ? null : new WeakReference(cont);
                    TaskScheduler = DataContextServices.Scheduler;
                }
                else if(target != null)
                {
                    DataContext = null;
                    DataContextServices.RegisterHandler(target, this);
                }
            }
        }
        
        protected virtual void DataContextChanged() { }
        
        private readonly TaskCompletionSource<object> _task;
        private bool _simpleMode;
        
        public WeakReference<DependencyObject>? Source { get; }
        
        public DependencyObject? Target => Source?.TypedTarget();
        
        public WeakReference? DataContext { get; set; }


        public bool IsAlive => Source?.IsAlive() ?? false;


        public ITaskScheduler? TaskScheduler { get; set; }

        public void ExecuteAsync() => Task.Run(DataContextChanged);

        public void ExecuteSync() => DataContextChanged();

        public virtual bool Synchronize => true;


        public Task Task => _task.Task;
        
        ITask IPipeLine.Generate(WeakReference dataContext, ITaskScheduler scheduler)
        {
            DataContext = dataContext;
            TaskScheduler = scheduler;
            return this;
        }

    }
}