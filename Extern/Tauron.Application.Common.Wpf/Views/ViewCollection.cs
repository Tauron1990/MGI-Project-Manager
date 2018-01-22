using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media;
using JetBrains.Annotations;
using Tauron.Application.Models;

namespace Tauron.Application.Views
{
    [PublicAPI]
    public class ViewCollection : ICollection<ViewModelBase>, INotifyCollectionChanged
    {
        public IEnumerable<DependencyObject> Views => _viewsInternal.Values;

        private ObservableDictionary<ViewModelBase, DependencyObject> _viewsInternal;

        public ViewCollection()
        {
            _viewsInternal = new ObservableDictionary<ViewModelBase, DependencyObject>();
        }

        public IEnumerator<ViewModelBase> GetEnumerator()
        {
            return _viewsInternal.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ViewModelBase item)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(ViewModelBase item)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(ViewModelBase[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(ViewModelBase item)
        {
            throw new System.NotImplementedException();
        }

        public int Count { get; }
        public bool IsReadOnly { get; }
        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}