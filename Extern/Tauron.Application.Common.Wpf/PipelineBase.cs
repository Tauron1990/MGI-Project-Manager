using System;
using System.Threading.Tasks;
using System.Windows;
using JetBrains.Annotations;

namespace Tauron.Application
{
    internal abstract class PipelineBase : IPipeLine, ITask
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PipelineBase" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="PipelineBase" /> Klasse.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        protected PipelineBase([NotNull] DependencyObject target, bool simpleMode)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            Source = new WeakReference<DependencyObject>(target);
            SimpleMode = simpleMode;
            _task = new TaskCompletionSource<object>();
            _task.SetResult(null);
        }

        #endregion

        public bool SimpleMode
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
                    TaskScheduler = CommonApplication.Scheduler;
                }
                else
                {
                    DataContext = null;
                    DataContextServices.RegisterHandler(target, this);
                }
            }
        }

        #region Methods

        /// <summary>The data context changed.</summary>
        protected virtual void DataContextChanged()
        {
        }

        #endregion

        #region Fields

        private readonly TaskCompletionSource<object> _task;
        private bool _simpleMode;

        #endregion

        #region Public Properties

        /// <summary>Gets the source.</summary>
        [CanBeNull]
        public WeakReference<DependencyObject> Source { get; }

        /// <summary>Gets the target.</summary>
        [CanBeNull]
        public DependencyObject Target => Source.TypedTarget();

        /// <summary>Gets or sets the data context.</summary>
        public WeakReference DataContext { get; set; }

        /// <summary>Gets a value indicating whether is alive.</summary>
        public bool IsAlive => Source.IsAlive();

        /// <summary>Gets or sets the task scheduler.</summary>
        [NotNull]
        public TaskScheduler TaskScheduler { get; set; }

        /// <summary>Gets a value indicating whether synchronize.</summary>
        public virtual bool Synchronize => true;

        /// <summary>Gets the task.</summary>
        [NotNull]
        public Task Task => _task.Task;

        #endregion

        #region Explicit Interface Methods

        ITask IPipeLine.Generate(WeakReference dataContext, TaskScheduler scheduler)
        {
            DataContext = dataContext;
            TaskScheduler = scheduler;
            return this;
        }

        void ITask.Execute()
        {
            DataContextChanged();
        }

        #endregion
    }
}