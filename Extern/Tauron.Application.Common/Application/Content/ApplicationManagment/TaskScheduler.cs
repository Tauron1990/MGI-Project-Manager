#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskScheduler.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The task scheduler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    public interface ITaskScheduler : IDisposable
    {
        /// <summary>
        ///     The queue task.
        /// </summary>
        /// <param name="task">
        ///     The task.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        Task QueueTask([NotNull] ITask task);
    }

    /// <summary>The task scheduler.</summary>
    [PublicAPI]
    public sealed class TaskScheduler : ITaskScheduler
    {
        #region Public Properties

        /// <summary></summary>
        /// <value>The disposed.</value>
        public bool Disposed => _disposed;

        #endregion

        #region Fields

        /// <summary>The _collection.</summary>
        private readonly BlockingCollection<ITask> _collection;

        /// <summary>The _synchronization context.</summary>
        private readonly IUISynchronize _synchronizationContext;

        /// <summary>The _disposed.</summary>
        private bool _disposed;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TaskScheduler" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="TaskScheduler" /> Klasse.
        ///     Initializes a new instance of the <see cref="TaskScheduler" /> class.
        /// </summary>
        /// <param name="synchronizationContext">
        ///     The synchronization context.
        /// </param>
        public TaskScheduler([NotNull] IUISynchronize synchronizationContext)
        {
            _synchronizationContext =
                synchronizationContext ?? throw new ArgumentNullException(nameof(synchronizationContext));
            _collection = new BlockingCollection<ITask>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TaskScheduler" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="TaskScheduler" /> Klasse.
        ///     Initializes a new instance of the <see cref="TaskScheduler" /> class.
        /// </summary>
        public TaskScheduler()
        {
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="TaskScheduler" /> class.
        ///     Finalisiert eine Instanz der <see cref="TaskScheduler" /> Klasse.
        ///     Finalizes an instance of the <see cref="TaskScheduler" /> class.
        /// </summary>
        ~TaskScheduler()
        {
            Dispose(false);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     The queue task.
        /// </summary>
        /// <param name="task">
        ///     The task.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [NotNull]
        public Task QueueTask([NotNull] ITask task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            CheckDispose();
            if (task.Synchronize && _synchronizationContext != null)
                return _synchronizationContext.BeginInvoke(task.Execute);

            if (_collection == null)
            {
                CommonConstants.LogCommon(false, "Task Scheduler: Scheduler Not Initialized");
                task.Execute();
                var tcs = new TaskCompletionSource<object>();
                tcs.SetResult(null);
                return tcs.Task;
            }

            _collection.Add(task);
            return task.Task;
        }

        #endregion

        #region Methods

        internal void EnterLoop()
        {
            foreach (var task in _collection.GetConsumingEnumerable()) task.Execute();

            _collection.Dispose();
        }

        /// <summary>The check dispose.</summary>
        /// <exception cref="ObjectDisposedException"></exception>
        private void CheckDispose()
        {
            if (_disposed) throw new ObjectDisposedException("TaskScheduler");
        }

        /// <summary>
        ///     The dispose.
        /// </summary>
        /// <param name="disposing">
        ///     The disposing.
        /// </param>
        [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_collection")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "disposing")]
        // ReSharper disable UnusedParameter.Local
        private void Dispose(bool disposing)
        {
            // ReSharper restore UnusedParameter.Local
            if (_disposed) return;

            _disposed = true;

            _collection?.CompleteAdding();
        }

        #endregion
    }
}