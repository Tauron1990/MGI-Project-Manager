using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using JetBrains.Annotations;

namespace Akka.Copy.App.Helper
{
    //[DebuggerNonUserCode]
    [PublicAPI]
    [Serializable]
    public class UiObservableCollection<TType> : ObservableCollection<TType>
    {
        private bool _isBlocked;

        private Dispatcher _synchronize;

        public UiObservableCollection(){}

        public UiObservableCollection([NotNull] IEnumerable<TType> enumerable)
            : base(enumerable){}

        [NotNull]
        protected Dispatcher InternalUiSynchronize
        {
            get
            {
                if (_synchronize != null) return _synchronize;
                _synchronize = Application.Current.Dispatcher;

                return _synchronize;
            }
        }

        public void AddRange([NotNull] IEnumerable<TType> enumerable)
        {
            if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));
            foreach (var item in enumerable) Add(item);
        }

        public IDisposable BlockChangedMessages() => new DispoableBlocker(this);

        private class DispoableBlocker : IDisposable
        {
            private readonly UiObservableCollection<TType> _collection;

            public DispoableBlocker(UiObservableCollection<TType> collection)
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

       
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_isBlocked) return;
            if (InternalUiSynchronize.CheckAccess())
                base.OnCollectionChanged(e);
            InternalUiSynchronize.Invoke(() => base.OnCollectionChanged(e));
        }
        
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (_isBlocked) return;
            if (InternalUiSynchronize.CheckAccess()) base.OnPropertyChanged(e);
            InternalUiSynchronize.Invoke(() => base.OnPropertyChanged(e));
        }
    }
}