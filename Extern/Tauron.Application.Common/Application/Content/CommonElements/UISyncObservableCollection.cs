#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>
    ///     The ui sync observable collection.
    /// </summary>
    /// <typeparam name="TType">
    /// </typeparam>
    [DebuggerNonUserCode]
    [PublicAPI]
    [Serializable]
    public class UISyncObservableCollection<TType> : ObservableCollection<TType>
    {
        private class DispoableBlocker : IDisposable
        {
            private readonly UISyncObservableCollection<TType> _collection;

            public DispoableBlocker(UISyncObservableCollection<TType> collection)
            {
                _collection = collection;
                _collection._isBlocked = true;
            }

            public void Dispose()
            {
                _collection._isBlocked = false;
                _collection.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        private class DummySync : IUISynchronize
        {
            public Task BeginInvoke(Action action)
            {
                action();
// ReSharper disable once AssignNullToNotNullAttribute
                return null;
            }

            public Task<TResult> BeginInvoke<TResult>(Func<TResult> action)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                return null;
            }

            public void Invoke(Action action)
            {
                action();
            }

            public TReturn Invoke<TReturn>(Func<TReturn> action)
            {
                return action();
            }
        }

        private bool _isBlocked;

        private IUISynchronize _synchronize;

        public UISyncObservableCollection()
        {
        }

        public UISyncObservableCollection([NotNull] IEnumerable<TType> enumerable)
            : base(enumerable)
        {
        }

        [NotNull]
        private IUISynchronize InternalUISynchronize
        {
            get
            {
                if (_synchronize != null) return _synchronize;
                _synchronize = UiSynchronize.Synchronize ?? new DummySync();

                return _synchronize;
            }
        }

        public void AddRange([NotNull] IEnumerable<TType> enumerable)
        {
            if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));
            foreach (var item in enumerable) Add(item);
        }

        public IDisposable BlockChangedMessages()
        {
            return new DispoableBlocker(this);
        }

        #region Methods

        /// <summary>
        ///     The on collection changed.
        /// </summary>
        /// <param name="e">
        ///     The e.
        /// </param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_isBlocked) return;
            InternalUISynchronize.Invoke(() => base.OnCollectionChanged(e));
        }

        /// <summary>
        ///     The on property changed.
        /// </summary>
        /// <param name="e">
        ///     The e.
        /// </param>
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (_isBlocked) return;
            InternalUISynchronize.Invoke(() => base.OnPropertyChanged(e));
        }

        #endregion
    }
}