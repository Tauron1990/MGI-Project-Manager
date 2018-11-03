#region

using System;
using System.Collections.Concurrent;
using System.Threading;
using JetBrains.Annotations;
using Tauron.Application.Ioc;

#endregion

namespace Tauron.Application.Implement
{
    /// <summary>The single thread scheduler.</summary>
    [PublicAPI]
    [Export(typeof(ISingleThreadScheduler))]
    [NotShared]
    public sealed class SingleThreadScheduler : ISingleThreadScheduler, IDisposable
    {
        #region Public Properties

        /// <summary>Gets or sets a value indicating whether is background.</summary>
        /// <value>The is background.</value>
        public bool IsBackground
        {
            get => _thread.IsBackground;

            set => _thread.IsBackground = value;
        }

        #endregion

        #region Methods

        /// <summary>The enter loop.</summa>
        private void EnterLoop()
        {
            foreach (var item in _tasks.GetConsumingEnumerable()) item();
        }

        #endregion

        #region Fields

        /// <summary>The _tasks.</summary>
        private readonly BlockingCollection<Action> _tasks = new BlockingCollection<Action>();

        /// <summary>The _thread.</summary>
        private readonly Thread _thread;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SingleThreadScheduler" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="SingleThreadScheduler" /> Klasse.
        ///     Initializes a new instance of the <see cref="SingleThreadScheduler" /> class.
        /// </summary>
        public SingleThreadScheduler()
        {
            _thread = new Thread(EnterLoop);
            IsBackground = true;

            _thread.Start();
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="SingleThreadScheduler" /> class.
        ///     Finalisiert eine Instanz der <see cref="SingleThreadScheduler" /> Klasse.
        ///     Finalizes an instance of the <see cref="SingleThreadScheduler" /> class.
        /// </summary>
        ~SingleThreadScheduler()
        {
            Dispose();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            _tasks.CompleteAdding();
            _tasks.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     The queue.
        /// </summary>
        /// <param name="task">
        ///     The task.
        /// </param>
        public void Queue(Action task)
        {
            _tasks.Add(task);
        }

        #endregion
    }
}