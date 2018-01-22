#region

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

#endregion

namespace Tauron.Application
{
    /// <summary>
    ///     The observable dictionary.
    /// </summary>
    /// <typeparam name="TKey">
    /// </typeparam>
    /// <typeparam name="TValue">
    /// </typeparam>
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    [DebuggerNonUserCode]
    [Serializable]
    public sealed class ObservableDictionary<TKey, TValue> : ObservableObject,
        IDictionary<TKey, TValue>,
        INotifyCollectionChanged
    {
        /// <summary>The block helper.</summary>
        [DebuggerNonUserCode]
        private class BlockHelper : IDisposable
        {
            #region Public Methods and Operators

            /// <summary>The dispose.</summary>
            public void Dispose()
            {
                Monitor.Exit(this);
            }

            /// <summary>The enter.</summary>
            public void Enter()
            {
                Monitor.Enter(this);
            }

            #endregion
        }

        /// <summary>The entry.</summary>
        [Serializable]
        [DebuggerNonUserCode]
        private class Entry
        {
            #region Fields

            /// <summary>The key.</summary>
            public TKey Key;

            /// <summary>The value.</summary>
            public TValue Value;

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     The construct.
            /// </summary>
            /// <param name="key">
            ///     The key.
            /// </param>
            /// <param name="value">
            ///     The value.
            /// </param>
            /// <returns>
            ///     The
            ///     <see>
            ///         <cref>KeyValuePair</cref>
            ///     </see>
            ///     .
            /// </returns>
            public static KeyValuePair<TKey, TValue> Construct(TKey key, TValue value)
            {
                return new KeyValuePair<TKey, TValue>(key, value);
            }

            /// <summary>
            ///     The construct.
            /// </summary>
            /// <param name="entry">
            ///     The entry.
            /// </param>
            /// <returns>
            ///     The
            ///     <see>
            ///         <cref>KeyValuePair</cref>
            ///     </see>
            ///     .
            /// </returns>
            public static KeyValuePair<TKey, TValue> Construct([NotNull] Entry entry)
            {
                if (entry == null) throw new ArgumentNullException(nameof(entry));
                return new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
            }

            #endregion
        }

        /// <summary>The key collection.</summary>
        [Serializable]
        [DebuggerNonUserCode]
        private class KeyCollection : NotifyCollectionChangedBase<TKey>
        {
            [NotNull]
            private readonly ObservableDictionary<TKey, TValue> _collection;

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="KeyCollection" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="KeyCollection" /> Klasse.
            ///     Initializes a new instance of the <see cref="KeyCollection" /> class.
            /// </summary>
            /// <param name="collection">
            ///     The collection.
            /// </param>
            public KeyCollection([NotNull] ObservableDictionary<TKey, TValue> collection)
                : base(collection)
            {
                _collection = collection;
            }

            #endregion

            #region Methods

            /// <summary>
            ///     The contains.
            /// </summary>
            /// <param name="entry">
            ///     The entry.
            /// </param>
            /// <param name="target">
            ///     The target.
            /// </param>
            /// <returns>
            ///     The <see cref="bool" />.
            /// </returns>
            [SuppressMessage("Microsoft.Design", "CA1062:Argumente von öffentlichen Methoden validieren",
                MessageId = "0")]
            protected override bool Contains(Entry entry, TKey target)
            {
                return Dictionary._keyEquals.Equals(entry.Key, target);
            }

            /// <summary>
            ///     The select.
            /// </summary>
            /// <param name="entry">
            ///     The entry.
            /// </param>
            /// <returns>
            ///     The <see cref="TKey" />.
            /// </returns>
            [SuppressMessage("Microsoft.Design", "CA1062:Argumente von öffentlichen Methoden validieren",
                MessageId = "0")]
            protected override TKey Select(Entry entry)
            {
                return entry.Key;
            }

            #endregion
        }

        /// <summary>
        ///     The notify collection changed base.
        /// </summary>
        /// <typeparam name="TTarget">
        /// </typeparam>
        [Serializable]
        [DebuggerNonUserCode]
        private abstract class NotifyCollectionChangedBase<TTarget> : ObservableObject,
            ICollection<TTarget>,
            INotifyCollectionChanged
        {
            #region Fields

            /// <summary>The dictionary.</summary>
            protected readonly ObservableDictionary<TKey, TValue> Dictionary;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="NotifyCollectionChangedBase{TTarget}" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="NotifyCollectionChangedBase{TTarget}" /> Klasse.
            ///     Initializes a new instance of the <see cref="NotifyCollectionChangedBase{TTarget}" /> class.
            /// </summary>
            /// <param name="dictionary">
            ///     The dictionary.
            /// </param>
            protected NotifyCollectionChangedBase([NotNull] ObservableDictionary<TKey, TValue> dictionary)
            {
                Dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            }

            #endregion

            #region Explicit Interface Methods

            /// <summary>The get enumerator.</summary>
            /// <returns>
            ///     The <see cref="IEnumerator" />.
            /// </returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

            #region Public Events

            /// <summary>The collection changed.</summary>
            public event NotifyCollectionChangedEventHandler CollectionChanged;

            #endregion

            #region Public Properties

            /// <summary>Gets the count.</summary>
            /// <value>The count.</value>
            public int Count => Dictionary.Count;

            /// <summary>Gets a value indicating whether is read only.</summary>
            /// <value>The is read only.</value>
            public bool IsReadOnly => true;

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     The add.
            /// </summary>
            /// <param name="item">
            ///     The item.
            /// </param>
            /// <exception cref="NotSupportedException">
            /// </exception>
            public void Add(TTarget item)
            {
                throw new NotSupportedException();
            }

            /// <summary>The clear.</summary>
            /// <exception cref="NotSupportedException"></exception>
            public void Clear()
            {
                throw new NotSupportedException();
            }

            /// <summary>
            ///     The contains.
            /// </summary>
            /// <param name="item">
            ///     The item.
            /// </param>
            /// <returns>
            ///     The <see cref="bool" />.
            /// </returns>
            public bool Contains(TTarget item)
            {
                return Dictionary._entrys.Any(ent => Contains(ent, item));
            }

            /// <summary>
            ///     The copy to.
            /// </summary>
            /// <param name="array">
            ///     The array.
            /// </param>
            /// <param name="arrayIndex">
            ///     The array index.
            /// </param>
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

            /// <summary>The get enumerator.</summary>
            /// <returns>
            ///     The <see cref="IEnumerator" />.
            /// </returns>
            /// <exception cref="InvalidOperationException"></exception>
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

            /// <summary>
            ///     The remove.
            /// </summary>
            /// <param name="item">
            ///     The item.
            /// </param>
            /// <returns>
            ///     The <see cref="bool" />.
            /// </returns>
            /// <exception cref="NotSupportedException">
            /// </exception>
            public bool Remove(TTarget item)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            ///     The on collection add.
            /// </summary>
            /// <param name="target">
            ///     The target.
            /// </param>
            /// <param name="index">
            ///     The index.
            /// </param>
            public void OnCollectionAdd(TTarget target, int index)
            {
                InvokeCollectionChanged(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, target,
                        index));
            }

            /// <summary>
            ///     The on collection remove.
            /// </summary>
            /// <param name="target">
            ///     The target.
            /// </param>
            /// <param name="index">
            ///     The index.
            /// </param>
            public void OnCollectionRemove(TTarget target, int index)
            {
                InvokeCollectionChanged(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                        target, index));
            }

            /// <summary>
            ///     The on collection replace.
            /// </summary>
            /// <param name="newItem">
            ///     The new item.
            /// </param>
            /// <param name="oldItem">
            ///     The old item.
            /// </param>
            /// <param name="index">
            ///     The index.
            /// </param>
            public void OnCollectionReplace(TTarget newItem, TTarget oldItem, int index)
            {
                InvokeCollectionChanged(
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                        newItem, oldItem, index));
            }

            /// <summary>The on collection reset.</summary>
            public void OnCollectionReset()
            {
                InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            #endregion

            #region Methods

            /// <summary>
            ///     The contains.
            /// </summary>
            /// <param name="entry">
            ///     The entry.
            /// </param>
            /// <param name="target">
            ///     The target.
            /// </param>
            /// <returns>
            ///     The <see cref="bool" />.
            /// </returns>
            protected abstract bool Contains([NotNull] Entry entry, TTarget target);

            /// <summary>
            ///     The select.
            /// </summary>
            /// <param name="entry">
            ///     The entry.
            /// </param>
            /// <returns>
            ///     The <see cref="TTarget" />.
            /// </returns>
            protected abstract TTarget Select([NotNull] Entry entry);

            /// <summary>
            ///     The invoke collection changed.
            /// </summary>
            /// <param name="e">
            ///     The e.
            /// </param>
            private void InvokeCollectionChanged([NotNull] NotifyCollectionChangedEventArgs e)
            {
                CurrentDispatcher.Invoke(() =>
                {
                    InvokePropertyChanged();
                    CollectionChanged?.Invoke(this, e);
                });
            }

            /// <summary>The invoke property changed.</summary>
            private void InvokePropertyChanged()
            {
                OnPropertyChangedExplicit("Item[]");
                OnPropertyChangedExplicit("Count");
                OnPropertyChangedExplicit("Keys");
                OnPropertyChangedExplicit("Values");
            }

            #endregion
        }

        /// <summary>The value collection.</summary>
        [Serializable]
        [DebuggerNonUserCode]
        private class ValueCollection : NotifyCollectionChangedBase<TValue>
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="ValueCollection" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="ValueCollection" /> Klasse.
            ///     Initializes a new instance of the <see cref="ValueCollection" /> class.
            /// </summary>
            /// <param name="collection">
            ///     The collection.
            /// </param>
            public ValueCollection([NotNull] ObservableDictionary<TKey, TValue> collection)
                : base(collection)
            {
                if (collection == null) throw new ArgumentNullException(nameof(collection));
            }

            #endregion

            #region Methods

            /// <summary>
            ///     The contains.
            /// </summary>
            /// <param name="entry">
            ///     The entry.
            /// </param>
            /// <param name="target">
            ///     The target.
            /// </param>
            /// <returns>
            ///     The <see cref="bool" />.
            /// </returns>
            [SuppressMessage("Microsoft.Design", "CA1062:Argumente von öffentlichen Methoden validieren",
                MessageId = "0")]
            protected override bool Contains(Entry entry, TValue target)
            {
                return EqualityComparer<TValue>.Default.Equals(entry.Value, target);
            }

            /// <summary>
            ///     The select.
            /// </summary>
            /// <param name="entry">
            ///     The entry.
            /// </param>
            /// <returns>
            ///     The <see cref="TValue" />.
            /// </returns>
            [SuppressMessage("Microsoft.Design", "CA1062:Argumente von öffentlichen Methoden validieren",
                MessageId = "0")]
            protected override TValue Select(Entry entry)
            {
                return entry.Value;
            }

            #endregion
        }

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObservableDictionary{TKey,TValue}" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ObservableDictionary{TKey,TValue}" /> Klasse.
        ///     Initializes a new instance of the <see cref="ObservableDictionary{TKey,TValue}" /> class.
        /// </summary>
        public ObservableDictionary()
        {
            _helper = new BlockHelper();
            _version = 1;
            _entrys = new Entry[4];
            _keyEquals = EqualityComparer<TKey>.Default;
            _keys = new KeyCollection(this);
            _values = new ValueCollection(this);
        }

        #endregion

        #region Explicit Interface Properties

        /// <summary>Gets a value indicating whether is read only.</summary>
        /// <value>The is read only.</value>
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        #endregion

        #region Public Indexers

        /// <summary>
        ///     The this.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <exception cref="KeyNotFoundException">
        /// </exception>
        /// <returns>
        ///     The <see cref="TValue" />.
        /// </returns>
        public TValue this[TKey key]
        {
            get
            {
                int index;
                var ent = FindEntry(key, out index);
                if (ent == null) throw new KeyNotFoundException(key.ToString());

                return ent.Value;
            }

            set
            {
                int index;
                var entry = FindEntry(key, out index);

                if (entry == null)
                {
                    AddCore(key, value);
                }
                else
                {
                    var temp = Entry.Construct(entry);
                    entry.Value = value;
                    OnCollectionReplace(Entry.Construct(entry), temp, index);
                }
            }
        }

        #endregion

        #region Public Events

        /// <summary>The collection changed.</summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region Fields

        /// <summary>The _entrys.</summary>
        private Entry[] _entrys;

        /// <summary>The _helper.</summary>
        [NonSerialized]
        private BlockHelper _helper;

        /// <summary>The _key equals.</summary>
        [NonSerialized]
        private IEqualityComparer<TKey> _keyEquals;

        /// <summary>The _keys.</summary>
        [NonSerialized]
        private KeyCollection _keys;

        /// <summary>The _values.</summary>
        [NonSerialized]
        private ValueCollection _values;

        /// <summary>The _version.</summary>
        [NonSerialized]
        private int _version;

        #endregion

        #region Public Properties

        /// <summary>Gets the count.</summary>
        /// <value>The count.</value>
        public int Count { get; private set; }

        /// <summary>Gets the keys.</summary>
        /// <value>The keys.</value>
        public ICollection<TKey> Keys => _keys;

        /// <summary>Gets the values.</summary>
        /// <value>The values.</value>
        public ICollection<TValue> Values => _values;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        public void Add(TKey key, TValue value)
        {
            int i;
            if (FindEntry(key, out i) != null) throw new ArgumentException("The key is in the collection unkown.");

            AddCore(key, value);
        }

        /// <summary>The clear.</summary>
        public void Clear()
        {
            Count = 0;
            Array.Clear(_entrys, 0, _entrys.Length);

            OnCollectionReset();
        }

        /// <summary>
        ///     The contains key.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool ContainsKey(TKey key)
        {
            int i;
            return FindEntry(key, out i) != null;
        }

        /// <summary>The get enumerator.</summary>
        /// <returns>
        ///     The <see cref="IEnumerator" />.
        /// </returns>
        /// <exception cref="InvalidOperationException"></exception>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var currver = _version;
            foreach (var t in _entrys.TakeWhile(t => t != null))
            {
                if (currver != _version) throw new InvalidOperationException();

                yield return Entry.Construct(t);
            }
        }

        /// <summary>
        ///     The remove.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool Remove(TKey key)
        {
            int index;
            var entry = FindEntry(key, out index);
            if (entry == null) return false;

            _entrys[index] = null;

            Array.Copy(_entrys, index + 1, _entrys, index, Count - index);
            Count--;

            OnCollectionRemove(Entry.Construct(entry), index);

            return true;
        }

        /// <summary>
        ///     The try get value.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            int index;
            var ent = FindEntry(key, out index);

            var flag = ent != null;

            value = flag ? ent.Value : default(TValue);

            return flag;
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        ///     The add.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        ///     The contains.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            int index;
            var ent = FindEntry(item.Key, out index);

            return ent != null && EqualityComparer<TValue>.Default.Equals(ent.Value, item.Value);
        }

        /// <summary>
        ///     The copy to.
        /// </summary>
        /// <param name="array">
        ///     The array.
        /// </param>
        /// <param name="arrayIndex">
        ///     The array index.
        /// </param>
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

        /// <summary>The get enumerator.</summary>
        /// <returns>
        ///     The <see cref="IEnumerator" />.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     The remove.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The add core.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        private void AddCore(TKey key, TValue value)
        {
            var index = Count;
            Count++;
            EnsureCapatcity(Count);

            _entrys[index] = new Entry {Key = key, Value = value};
            OnCollectionAdd(Entry.Construct(key, value), index);
        }

        /// <summary>The block collection.</summary>
        /// <returns>
        ///     The <see cref="IDisposable" />.
        /// </returns>
        [NotNull]
        private IDisposable BlockCollection()
        {
            _helper.Enter();
            return _helper;
        }

        /// <summary>
        ///     The ensure capatcity.
        /// </summary>
        /// <param name="min">
        ///     The min.
        /// </param>
        private void EnsureCapatcity(int min)
        {
            if (_entrys.Length < min) Array.Resize(ref _entrys, Convert.ToInt32(_entrys.Length * 2));
        }

        /// <summary>
        ///     The find entry.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <param name="index">
        ///     The index.
        /// </param>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>ObservableDictionary</cref>
        ///     </see>
        ///     .
        /// </returns>
        [CanBeNull]
        private Entry FindEntry(TKey key, out int index)
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

        /// <summary>
        ///     The invoke collection changed.
        /// </summary>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void InvokeCollectionChanged([NotNull] NotifyCollectionChangedEventArgs e)
        {
            _version++;

            CurrentDispatcher.Invoke(() =>
            {
                CollectionChanged?.Invoke(this, e);
            });
        }

        /// <summary>The invoke property changed.</summary>
        private void InvokePropertyChanged()
        {
            CurrentDispatcher.Invoke(() =>
            {
                OnPropertyChangedExplicit("Item[]");
                OnPropertyChangedExplicit("Count");
                OnPropertyChangedExplicit("Keys");
                OnPropertyChangedExplicit("Values");
            });
        }

        /// <summary>
        ///     The on collection add.
        /// </summary>
        /// <param name="changed">
        ///     The changed.
        /// </param>
        /// <param name="index">
        ///     The index.
        /// </param>
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

        /// <summary>
        ///     The on collection remove.
        /// </summary>
        /// <param name="changed">
        ///     The changed.
        /// </param>
        /// <param name="index">
        ///     The index.
        /// </param>
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

        /// <summary>
        ///     The on collection replace.
        /// </summary>
        /// <param name="newItem">
        ///     The new item.
        /// </param>
        /// <param name="oldItem">
        ///     The old item.
        /// </param>
        /// <param name="index">
        ///     The index.
        /// </param>
        private void OnCollectionReplace(
            KeyValuePair<TKey, TValue> newItem,
            KeyValuePair<TKey, TValue> oldItem,
            int index)
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

        /// <summary>The on collection reset.</summary>
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

        /// <summary>
        ///     The on deserialized.
        /// </summary>
        /// <param name="context">
        ///     The context.
        /// </param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            _helper = new BlockHelper();
            _version = 1;
            _keyEquals = EqualityComparer<TKey>.Default;
            _keys = new KeyCollection(this);
            _values = new ValueCollection(this);
        }

        #endregion
    }
}