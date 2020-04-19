using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Catel.Collections;
using JetBrains.Annotations;

namespace Tauron.Application.Wpf.Helper
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class WeakCollection<TType> : IList<TType>
        where TType : class
    {
        private readonly List<WeakReference<TType>?> _internalCollection = new List<WeakReference<TType>?>();

        public WeakCollection()
        {
            WeakCleanUp.RegisterAction(CleanUp);
        }

        public int EffectiveCount => _internalCollection.Count(refer => refer?.IsAlive() ?? false);

        public TType? this[int index]
        {
#pragma warning disable CS8613 // Die NULL-Zulässigkeit von Verweistypen im Rückgabetyp entspricht nicht dem implizit implementierten Member.
#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
            get => _internalCollection[index]?.TypedTarget();
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
            set => _internalCollection[index] = value == null ? null : new WeakReference<TType>(value);
            #pragma warning restore CS8613 // Die NULL-Zulässigkeit von Verweistypen im Rückgabetyp entspricht nicht dem implizit implementierten Member.
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => _internalCollection.Count;

        public bool IsReadOnly => false;

        public void Add(TType item)
        {
            if (item == null) return;
            _internalCollection.Add(new WeakReference<TType>(item));
        }

        /// <summary>The clear.</summary>
        public void Clear()
        {
            _internalCollection.Clear();
        }

        public bool Contains(TType item)
        {
            return item != null && _internalCollection.Any(it => it?.TypedTarget() == item);
        }

        public void CopyTo(TType[] array, int arrayIndex)
        {
            Argument.NotNull(array, nameof(array));

            var index = 0;
            for (var i = arrayIndex; i < array.Length; i++)
            {
                TType? target = null;
                while (target == null && index <= _internalCollection.Count)
                {
                    target = _internalCollection[index]?.TypedTarget();
                    index++;
                }

                if (target == null) break;

                array[i] = target;
            }
        }

        public IEnumerator<TType> GetEnumerator()
        {
            return
                _internalCollection.Select(reference => reference?.TypedTarget())
                   .Where(target => target != null)
                   .GetEnumerator()!;
        }

        public int IndexOf(TType item)
        {
            if (item == null) return -1;

            int index;
            for (index = 0; index < _internalCollection.Count; index++)
            {
                var temp = _internalCollection[index];
                if (temp?.TypedTarget() == item) break;
            }

            return index == _internalCollection.Count ? -1 : index;
        }

        public void Insert(int index, TType item)
        {
            if (item == null) return;
            _internalCollection.Insert(index, new WeakReference<TType>(item));
        }

        public bool Remove(TType item)
        {
            if (item == null) return false;
            var index = IndexOf(item);
            if (index == -1) return false;

            _internalCollection.RemoveAt(index);
            return true;
        }

        public void RemoveAt(int index)
        {
            _internalCollection.RemoveAt(index);
        }

        public event EventHandler? CleanedEvent;

        internal void CleanUp()
        {
            var dead = _internalCollection.Where(reference => !reference?.IsAlive() ?? false).ToArray();
            foreach (var genericWeakReference in dead) _internalCollection.Remove(genericWeakReference);

            OnCleaned();
        }

        private void OnCleaned()
        {
            CleanedEvent?.Invoke(this, EventArgs.Empty);
        }
    }

    [DebuggerNonUserCode, PublicAPI]
    public class WeakReferenceCollection<TType> : Collection<TType>
        where TType : IInternalWeakReference
    {
        public WeakReferenceCollection()
        {
            WeakCleanUp.RegisterAction(CleanUpMethod);
        }

        protected override void ClearItems()
        {
            lock (this) base.ClearItems();
        }

        protected override void InsertItem(int index, TType item)
        {
            lock (this)
            {
                if (index > Count) index = Count;
                base.InsertItem(index, item);
            }
        }

        protected override void RemoveItem(int index)
        {
            lock (this) base.RemoveItem(index);
        }

        protected override void SetItem(int index, TType item)
        {
            lock (this) base.SetItem(index, item);
        }

        private void CleanUpMethod()
        {
            lock (this)
            {
                Items.ToArray()
                   .Where(it => !it.IsAlive)
                   .ForEach(it =>
                            {
                                if (it is IDisposable dis) dis.Dispose();

                                Items.Remove(it);
                            });
            }
        }
    }
}