using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;

namespace Tauron.Application.Models
{
    [PublicAPI]
    public class EditableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IEditableObject
    {
        private Dictionary<TKey, TValue> _backup;
        private Dictionary<TKey, TValue> _primary;

        public EditableDictionary()
        {
            _primary = new Dictionary<TKey, TValue>();
        }

        public bool BlockIsNotEditing { get; set; }
        public bool IsEditing { get; private set; }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            if (IsEditing)
                return _backup.GetEnumerator();
            return _primary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            if (EnsureState())
                return;

            GetCollection().Add(item);
        }

        public void Clear()
        {
            if (EnsureState())
                return;
            _primary.Clear();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return GetCollection().Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            GetCollection().CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (EnsureState()) return false;

            return GetCollection().Remove(item);
        }

        public int Count => _primary.Count;

        public bool IsReadOnly
        {
            get
            {
                if (BlockIsNotEditing) return !IsEditing;
                return false;
            }
        }

        public bool ContainsKey(TKey key)
        {
            return _primary.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            if (EnsureState()) return;

            _primary.Add(key, value);
        }

        public bool Remove(TKey key)
        {
            return !EnsureState() && _primary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _primary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get => _primary[key];
            set
            {
                if (EnsureState()) return;

                _primary[key] = value;
            }
        }

        public ICollection<TKey> Keys => _primary.Keys;
        public ICollection<TValue> Values => _primary.Values;

        public void BeginEdit()
        {
            if (IsEditing) return;

            IsEditing = true;
            _backup = new Dictionary<TKey, TValue>(_primary);
        }

        public void EndEdit()
        {
            if (!IsEditing) return;

            IsEditing = false;
            _backup = null;
        }

        public void CancelEdit()
        {
            if (!IsEditing) return;

            IsEditing = false;

            _primary = _backup;
            _backup = null;
        }

        private bool EnsureState()
        {
            if (IsEditing) return false;

            if (BlockIsNotEditing) return true;

            return false;
        }

        private ICollection<KeyValuePair<TKey, TValue>> GetCollection()
        {
            return _primary.CastObj<ICollection<KeyValuePair<TKey, TValue>>>();
        }
    }
}