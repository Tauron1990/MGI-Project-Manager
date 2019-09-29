using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ServiceManager
{
    public sealed class ObservableQueue<T> : Queue<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public ObservableQueue()
        {
        }

        public ObservableQueue(IEnumerable<T> collection)
        {
            foreach (var item in collection)
                Enqueue(item);
        }

        public ObservableQueue(List<T> list)
        {
            foreach (var item in list)
                Enqueue(item);
        }


        public event NotifyCollectionChangedEventHandler CollectionChanged;


        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add => PropertyChanged += value;
            remove => PropertyChanged -= value;
        }


        public new void Clear()
        {
            base.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public new T Dequeue()
        {
            var item = base.Dequeue();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            return item;
        }

        public new void Enqueue(T item)
        {
            base.Enqueue(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }


        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            RaiseCollectionChanged(e);
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
        }

        private void OnPropertyChanged(PropertyChangedEventArgs e) => RaisePropertyChanged(e);


        private event PropertyChangedEventHandler PropertyChanged;


        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e) => CollectionChanged?.Invoke(this, e);

        private void RaisePropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);
    }
}