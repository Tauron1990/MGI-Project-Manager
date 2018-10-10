#region

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    public class UserResultTask<TResult> : ITask
    {
        public UserResultTask([NotNull] Func<TResult> callback, bool sync)
        {
            _callback   = callback ?? throw new ArgumentNullException(nameof(callback));
            Synchronize = sync;
            _task       = new TaskCompletionSource<TResult>();
        }

        #region Public Methods and Operators

        /// <summary>The execute.</summary>
        public void Execute()
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

        #endregion

        #region Fields

        /// <summary>The _callback.</summary>
        private readonly Func<TResult> _callback;

        private readonly TaskCompletionSource<TResult> _task;

        #endregion

        #region Public Properties

        /// <summary>Gets a value indicating whether synchronize.</summary>
        /// <value>The synchronize.</value>
        public bool Synchronize { get; }

        /// <summary>Gets the task.</summary>
        public Task Task => _task.Task;

        #endregion
    }

    /// <summary>The user task.</summary>
    public class UserTask : ITask
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserTask" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="UserTask" /> Klasse.
        ///     Initializes a new instance of the <see cref="UserTask" /> class.
        /// </summary>
        /// <param name="callback">
        ///     The callback.
        /// </param>
        /// <param name="sync">
        ///     The sync.
        /// </param>
        public UserTask([NotNull] Action callback, bool sync)
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));
            _callback   = callback;
            Synchronize = sync;
            _task       = new TaskCompletionSource<object>();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The execute.</summary>
        public void Execute()
        {
            try
            {
                _callback();
                _task.SetResult(null);
            }
            catch (Exception e)
            {
                _task.SetException(e);
            }
        }

        #endregion

        #region Fields

        /// <summary>The _callback.</summary>
        private readonly Action _callback;

        private readonly TaskCompletionSource<object> _task;

        #endregion

        #region Public Properties

        /// <summary>Gets a value indicating whether synchronize.</summary>
        /// <value>The synchronize.</value>
        public bool Synchronize { get; }

        /// <summary>Gets the task.</summary>
        public Task Task => _task.Task;

        #endregion
    }
}