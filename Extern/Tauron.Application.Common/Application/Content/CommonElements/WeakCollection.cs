#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>
    ///     The weak collection.
    /// </summary>
    /// <typeparam name="TType">
    /// </typeparam>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class WeakCollection<TType> : IList<TType>
        where TType : class
    {
        #region Fields

        /// <summary>The _internal collection.</summary>
        private readonly List<WeakReference<TType>> _internalCollection = new List<WeakReference<TType>>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeakCollection{TType}" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="WeakCollection{TType}" /> Klasse.
        ///     Initializes a new instance of the <see cref="WeakCollection{TType}" /> class.
        /// </summary>
        public WeakCollection()
        {
            WeakCleanUp.RegisterAction(CleanUp);
        }

        #endregion

        #region Public Indexers

        /// <summary>
        ///     The this.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        /// <returns>
        ///     The Value.
        /// </returns>
        public TType this[int index]
        {
            get => _internalCollection[index].TypedTarget();

            set => _internalCollection[index] = new WeakReference<TType>(value);
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

        /// <summary>The cleaned event.</summary>
        public event EventHandler CleanedEvent;

        #endregion

        #region Public Properties

        /// <summary>Gets the effective count.</summary>
        /// <value>The effective count.</value>
        public int EffectiveCount
        {
            get { return _internalCollection.Count(refer => refer.IsAlive()); }
        }

        /// <summary>Gets the count.</summary>
        /// <value>The count.</value>
        public int Count => _internalCollection.Count;

        /// <summary>Gets a value indicating whether is read only.</summary>
        /// <value>The is read only.</value>
        public bool IsReadOnly => false;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The add.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
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

        /// <summary>
        ///     The contains.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool Contains(TType item)
        {
            return item != null && _internalCollection.Any(it => it.TypedTarget() == item);
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
        public void CopyTo(TType[] array, int arrayIndex)
        {
            var index = 0;
            for (var i = arrayIndex; i < array.Length; i++)
            {
                TType target = null;
                while (target == null && index <= _internalCollection.Count)
                {
                    target = _internalCollection[index].TypedTarget();
                    index++;
                }

                if (target == null) break;

                array[i] = target;
            }
        }

        /// <summary>The get enumerator.</summary>
        /// <returns>
        ///     The <see cref="IEnumerator" />.
        /// </returns>
        public IEnumerator<TType> GetEnumerator()
        {
            return
                _internalCollection.Select(reference => reference.TypedTarget())
                    .Where(target => target != null)
                    .GetEnumerator();
        }

        /// <summary>
        ///     The index of.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public int IndexOf(TType item)
        {
            if (item == null) return -1;

            int index;
            for (index = 0; index < _internalCollection.Count; index++)
            {
                var temp = _internalCollection[index];
                if (temp.TypedTarget() == item) break;
            }

            return index == _internalCollection.Count ? -1 : index;
        }

        /// <summary>
        ///     The insert.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        /// <param name="item">
        ///     The item.
        /// </param>
        public void Insert(int index, TType item)
        {
            if (item == null) return;

            _internalCollection.Insert(index, new WeakReference<TType>(item));
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
        public bool Remove(TType item)
        {
            if (item == null) return false;

            var index = IndexOf(item);
            if (index == -1) return false;

            _internalCollection.RemoveAt(index);
            return true;
        }

        /// <summary>
        ///     The remove at.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        public void RemoveAt(int index)
        {
            _internalCollection.RemoveAt(index);
        }

        #endregion

        #region Methods

        /// <summary>The clean up.</summary>
        internal void CleanUp()
        {
            var dead = _internalCollection.Where(reference => !reference.IsAlive()).ToArray();

            foreach (var genericWeakReference in dead) _internalCollection.Remove(genericWeakReference);

            OnCleaned();
        }

        /// <summary>The on cleaned.</summary>
        private void OnCleaned()
        {
            CleanedEvent?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }

    /// <summary>
    ///     The weak reference collection.
    /// </summary>
    /// <typeparam name="TType">
    /// </typeparam>
    [DebuggerNonUserCode]
    public class WeakReferenceCollection<TType> : Collection<TType>
        where TType : IWeakReference
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeakReferenceCollection{TType}" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="WeakReferenceCollection{TType}" /> Klasse.
        ///     Initializes a new instance of the <see cref="WeakReferenceCollection{TType}" /> class.
        /// </summary>
        public WeakReferenceCollection()
        {
            WeakCleanUp.RegisterAction(CleanUpMethod);
        }

        #endregion

        #region Methods

        /// <summary>The clear items.</summary>
        protected override void ClearItems()
        {
            lock (this)
            {
                base.ClearItems();
            }
        }

        /// <summary>
        ///     The insert item.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        /// <param name="item">
        ///     The item.
        /// </param>
        protected override void InsertItem(int index, TType item)
        {
            lock (this)
            {
                if (index > Count) index = Count;

                base.InsertItem(index, item);
            }
        }

        /// <summary>
        ///     The remove item.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        protected override void RemoveItem(int index)
        {
            lock (this)
            {
                base.RemoveItem(index);
            }
        }

        /// <summary>
        ///     The set item.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        /// <param name="item">
        ///     The item.
        /// </param>
        protected override void SetItem(int index, TType item)
        {
            lock (this)
            {
                base.SetItem(index, item);
            }
        }

        /// <summary>The clean up method.</summary>
        private void CleanUpMethod()
        {
            lock (this)
            {
                Items.ToArray()
                    .Where(it => !it.IsAlive)
                    .ToArray()
                    .Foreach(
                        it =>
                        {
                            if (it is IDisposable dis) dis.Dispose();

                            Items.Remove(it);
                        });
            }
        }

        #endregion
    }
}