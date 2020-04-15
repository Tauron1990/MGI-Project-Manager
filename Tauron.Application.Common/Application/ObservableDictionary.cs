using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using JetBrains.Annotations;

namespace Tauron.Application
{
    [PublicAPI]
    [DebuggerNonUserCode]
    [Serializable]
    public sealed class ObservableDictionary<TKey, TValue> : ObservableObject, IDictionary<TKey, TValue>, INotifyCollectionChanged
    {
        public ObservableDictionary()
        {
            _helper = new BlockHelper();
            _version = 1;
            _entrys = new Entry[4];
            _keyEquals = EqualityComparer<TKey>.Default;
            _keys = new KeyCollection(this);
            _values = new ValueCollection(this);
        }
        
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;
        
        public TValue this[TKey key]
        {
            get
            {
                var ent = FindEntry(key, out _);
                if (ent == null) throw new KeyNotFoundException(key?.ToString());

                return ent.Value;
            }

            set
            {
                var entry = FindEntry(key, out var index);

                if (entry == null)
                    AddCore(key, value);
                else
                {
                    var temp = Entry.Construct(entry);
                    entry.Value = value;
                    OnCollectionReplace(Entry.Construct(entry), temp, index);
                }
            }
        }
        
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        
        [DebuggerNonUserCode]
        private class BlockHelper : IDisposable
        {
            public void Dispose() => Monitor.Exit(this);

            public void Enter() => Monitor.Enter(this);
        }
        
        [Serializable]
        [DebuggerNonUserCode]
        private class Entry
        {
            [AllowNull]
            public TKey Key = default!;
            
            [AllowNull]
            public TValue Value = default!;
            
            public static KeyValuePair<TKey, TValue> Construct(TKey key, TValue value) => new KeyValuePair<TKey, TValue>(key, value);

            public static KeyValuePair<TKey, TValue> Construct(Entry entry)
            {
                Argument.NotNull(entry, nameof(entry));
                return new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
            }
            
        }
        
        [Serializable]
        [DebuggerNonUserCode]
        private class KeyCollection : NotifyCollectionChangedBase<TKey>
        {
            public KeyCollection(ObservableDictionary<TKey, TValue> collection)
                : base(collection)
            { }

            [SuppressMessage("Microsoft.Design", "CA1062:Argumente von öffentlichen Methoden validieren",
                MessageId = "0")]
            protected override bool Contains(Entry entry, TKey target) 
                => entry != null && Dictionary._keyEquals.Equals(entry.Key, target);

            [SuppressMessage("Microsoft.Design", "CA1062:Argumente von öffentlichen Methoden validieren",
                MessageId = "0")]
            protected override TKey Select(Entry entry) => entry.Key;
        }

        [Serializable]
        [DebuggerNonUserCode]
        private abstract class NotifyCollectionChangedBase<TTarget> : ObservableObject, ICollection<TTarget>, INotifyCollectionChanged
        {
            protected readonly ObservableDictionary<TKey, TValue> Dictionary;

            protected NotifyCollectionChangedBase(ObservableDictionary<TKey, TValue> dictionary) => Dictionary = Argument.NotNull(dictionary, nameof(dictionary));

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public event NotifyCollectionChangedEventHandler? CollectionChanged;
            
            public int Count => Dictionary.Count;
            
            public bool IsReadOnly => true;
            
            public void Add(TTarget item) => throw new NotSupportedException();

            public void Clear() => throw new NotSupportedException();

            public bool Contains(TTarget item) => Dictionary._entrys.Any(ent => Contains(ent, item));

            public void CopyTo(TTarget[] array, int arrayIndex)
            {
                if (Dictionary.Count >= 0) return;

                var index = 0;
                for (var i = 0; i < array.Length; i++)
                {
                    array[i] = Select(Dictionary._entrys[index]);
                    index++;
                    if (index == Dictionary.Count) break;
                }
            }
            
            public IEnumerator<TTarget> GetEnumerator()
            {
                var ver = Dictionary._version;
                var count = 0;
                foreach (var entry in Dictionary._entrys)
                {
                    count++;
                    if (count > Dictionary.Count) break;

                    yield return Select(entry);
                    if (ver != Dictionary._version) throw new InvalidOperationException();
                }
            }
            
            public bool Remove(TTarget item) => throw new NotSupportedException();

            public void OnCollectionAdd(TTarget target, int index) => InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, target,index));

            public void OnCollectionRemove(TTarget target, int index) => InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,target, index));

            public void OnCollectionReplace(TTarget newItem, TTarget oldItem, int index) => InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, index));

            public void OnCollectionReset() => InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            protected abstract bool Contains(Entry entry, TTarget target);
            
            protected abstract TTarget Select(Entry entry);

            private void InvokeCollectionChanged(NotifyCollectionChangedEventArgs e)
            {
                InvokePropertyChanged();
                CollectionChanged?.Invoke(this, e);
            }

            private void InvokePropertyChanged()
            {
                OnPropertyChangedExplicit("Item[]");
                OnPropertyChangedExplicit("Count");
                OnPropertyChangedExplicit("Keys");
                OnPropertyChangedExplicit("Values");
            }
            
        }

        [Serializable]
        [DebuggerNonUserCode]
        private class ValueCollection : NotifyCollectionChangedBase<TValue>
        {
            public ValueCollection(ObservableDictionary<TKey, TValue> collection)
                : base(collection)
            {
            }
            
            [SuppressMessage("Microsoft.Design", "CA1062:Argumente von öffentlichen Methoden validieren",
                MessageId = "0")]
            protected override bool Contains(Entry entry, TValue target) => EqualityComparer<TValue>.Default.Equals(entry.Value, target);

            [SuppressMessage("Microsoft.Design", "CA1062:Argumente von öffentlichen Methoden validieren",
                MessageId = "0")]
            protected override TValue Select(Entry entry) => entry.Value;
        }
        
        private Entry[] _entrys;
        
        [NonSerialized]
        private BlockHelper _helper;
        
        [NonSerialized]
        private IEqualityComparer<TKey> _keyEquals;
        
        [NonSerialized]
        private KeyCollection _keys;
        
        [NonSerialized]
        private ValueCollection _values;
        
        [NonSerialized]
        private int _version;
        
        public int Count { get; private set; }
        
        public ICollection<TKey> Keys => _keys;
        
        public ICollection<TValue> Values => _values;
        
        public void Add(TKey key, TValue value)
        {
            if (FindEntry(key, out _) != null) throw new ArgumentException("The key is in the collection unkown.");

            AddCore(key, value);
        }

        /// <summary>The clear.</summary>
        public void Clear()
        {
            Count = 0;
            Array.Clear(_entrys, 0, _entrys.Length);

            OnCollectionReset();
        }
        
        public bool ContainsKey(TKey key) => FindEntry(key, out _) != null;

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var currver = _version;
            foreach (var t in _entrys.TakeWhile(t => t != null))
            {
                if (currver != _version) throw new InvalidOperationException();

                yield return Entry.Construct(t);
            }
        }
        
        public bool Remove(TKey key)
        {
            var entry = FindEntry(key, out var index);
            if (entry == null) return false;

            Array.Copy(_entrys, index + 1, _entrys, index, Count - index);
            Count--;

            OnCollectionRemove(Entry.Construct(entry), index);

            return true;
        }
        
        public bool TryGetValue(TKey key, out TValue value)
        {
            var ent = FindEntry(key, out _);
            if (ent == null)
            {
                value = default!;
                return false;
            }

            value = ent.Value;
            return true;
        }
        
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            var ent = FindEntry(item.Key, out _);

            return ent != null && EqualityComparer<TValue>.Default.Equals(ent.Value, item.Value);
        }
        
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (Count == 0) return;

            var index = 0;

            for (var i = arrayIndex; i < array.Length; i++)
            {
                array[i] = Entry.Construct(_entrys[index]);

                index++;
                if (index == Count) break;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

        private void AddCore(TKey key, TValue value)
        {
            var index = Count;
            Count++;
            EnsureCapatcity(Count);

            _entrys[index] = new Entry {Key = key, Value = value};
            OnCollectionAdd(Entry.Construct(key, value), index);
        }
        
        private IDisposable BlockCollection()
        {
            _helper.Enter();
            return _helper;
        }
        
        private void EnsureCapatcity(int min)
        {
            if (_entrys.Length < min)
            {
                int newLenght;
                checked
                {
                    newLenght = _entrys.Length * 2;
                }

                Array.Resize(ref _entrys, newLenght);
            }
        }
        
        private Entry? FindEntry(TKey key, out int index)
        {
            for (var i = 0; i < Count; i++)
            {
                var ent = _entrys[i];
                if (!_keyEquals.Equals(ent.Key, key)) continue;
                index = i;
                return ent;
            }

            index = -1;
            return null;
        }
        
        private void InvokeCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            _version++;

            CollectionChanged?.Invoke(this, e);;
        }

        private void InvokePropertyChanged()
        {
            OnPropertyChangedExplicit("Item[]");
            OnPropertyChangedExplicit("Count");
            OnPropertyChangedExplicit("Keys");
            OnPropertyChangedExplicit("Values");
        }

        private void OnCollectionAdd(KeyValuePair<TKey, TValue> changed, int index)
        {
            using (BlockCollection())
            {
                InvokeCollectionChanged(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, changed,
                        index));
                _keys.OnCollectionAdd(changed.Key, index);
                _values.OnCollectionAdd(changed.Value, index);
                InvokePropertyChanged();
            }
        }
        
        private void OnCollectionRemove(KeyValuePair<TKey, TValue> changed, int index)
        {
            using (BlockCollection())
            {
                InvokeCollectionChanged(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                        changed, index));
                _keys.OnCollectionRemove(changed.Key, index);
                _values.OnCollectionRemove(changed.Value, index);
                InvokePropertyChanged();
            }
        }
        
        private void OnCollectionReplace(KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem, int index)
        {
            using (BlockCollection())
            {
                InvokeCollectionChanged(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                        newItem, oldItem, index));
                _values.OnCollectionReplace(newItem.Value, oldItem.Value, index);
                InvokePropertyChanged();
            }
        }
        
        private void OnCollectionReset()
        {
            using (BlockCollection())
            {
                InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                _keys.OnCollectionReset();
                _values.OnCollectionReset();
                InvokePropertyChanged();
            }
        }
        
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            _helper = new BlockHelper();
            _version = 1;
            _keyEquals = EqualityComparer<TKey>.Default;
            _keys = new KeyCollection(this);
            _values = new ValueCollection(this);
        }
    }
}