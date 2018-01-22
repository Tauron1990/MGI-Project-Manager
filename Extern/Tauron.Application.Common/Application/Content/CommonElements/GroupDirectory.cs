#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application
{
    /// <summary>
    ///     Eine Klasse die eine Liste von Objecten einem Schlüssel zuordnent.
    /// </summary>
    /// <typeparam name="TKey">
    ///     Der Type des Schlüssels.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     Der Type des Inhalts.
    /// </typeparam>
    [Serializable]
    [PublicAPI]
    public class GroupDictionary<TKey, TValue> : Dictionary<TKey, ICollection<TValue>>
        where TKey : class where TValue : class
    {
        #region Nested type: AllValueCollection

        /// <summary>The all value collection.</summary>
        private class AllValueCollection : ICollection<TValue>
        {
            #region Fields

            /// <summary>The _list.</summary>
            private readonly GroupDictionary<TKey, TValue> _list;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="AllValueCollection" /> class.
            ///     Initialisiert eine neue Instanz der <see cref="AllValueCollection" /> Klasse.
            ///     Initializes a new instance of the <see cref="AllValueCollection" /> class.
            /// </summary>
            /// <param name="list">
            ///     The list.
            /// </param>
            public AllValueCollection([NotNull] GroupDictionary<TKey, TValue> list)
            {
                if (list == null) throw new ArgumentNullException(nameof(list));
                _list = list;
            }

            #endregion

            #region Properties

            /// <summary>Gets the get all.</summary>
            /// <value>The get all.</value>
            [NotNull]
            private IEnumerable<TValue> GetAll
            {
                get { return _list.SelectMany(pair => pair.Value); }
            }

            #endregion

            #region Explicit Interface Methods

            /// <summary>Gibt einen Enumerator zurück, der eine Auflistung durchläuft.</summary>
            /// <returns>
            ///     Ein <see cref="T:System.Collections.IEnumerator" />-Objekt, das zum Durchlaufen der Auflistung verwendet werden
            ///     kann.
            /// </returns>
            /// <filterpriority>2</filterpriority>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Ruft die Anzahl der Elemente ab, die in <see cref="T:System.Collections.Generic.ICollection`1" /> enthalten sind.
            /// </summary>
            /// <returns>
            ///     Die Anzahl der Elemente, die in <see cref="T:System.Collections.Generic.ICollection`1" /> enthalten sind.
            /// </returns>
            /// <value>The count.</value>
            public int Count => GetAll.Count();

            /// <summary>
            ///     Ruft einen Wert ab, der angibt, ob <see cref="T:System.Collections.Generic.ICollection`1" /> schreibgeschützt ist.
            /// </summary>
            /// <returns>
            ///     True, wenn <see cref="T:System.Collections.Generic.ICollection`1" /> schreibgeschützt ist, andernfalls false.
            /// </returns>
            /// <value>The is read only.</value>
            public bool IsReadOnly => true;

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     Fügt der <see cref="T:System.Collections.Generic.ICollection`1" /> ein Element hinzu.
            /// </summary>
            /// <param name="item">
            ///     Das Objekt, das <see cref="T:System.Collections.Generic.ICollection`1" /> hinzugefügt werden soll.
            /// </param>
            /// <exception cref="T:System.NotSupportedException">
            ///     <see cref="T:System.Collections.Generic.ICollection`1" /> ist schreibgeschützt.
            /// </exception>
            public void Add([NotNull] TValue item)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            ///     Entfernt alle Elemente aus <see cref="T:System.Collections.Generic.ICollection`1" />.
            /// </summary>
            /// <exception cref="T:System.NotSupportedException">
            ///     <see cref="T:System.Collections.Generic.ICollection`1" /> ist schreibgeschützt.
            /// </exception>
            public void Clear()
            {
                throw new NotSupportedException();
            }

            /// <summary>
            ///     Bestimmt, ob <see cref="T:System.Collections.Generic.ICollection`1" /> einen bestimmten Wert enthält.
            /// </summary>
            /// <returns>
            ///     True, wenn sich <paramref name="item" /> in <see cref="T:System.Collections.Generic.ICollection`1" /> befindet,
            ///     andernfalls false.
            /// </returns>
            /// <param name="item">
            ///     Das im <see cref="T:System.Collections.Generic.ICollection`1" /> zu suchende Objekt.
            /// </param>
            [ContractVerification(false)]
            public bool Contains(TValue item)
            {
                return GetAll.Contains(item);
            }

            /// <summary>
            ///     Kopiert die Elemente von <see cref="T:System.Collections.Generic.ICollection`1" /> in ein
            ///     <see cref="T:System.Array" />
            ///     , beginnend bei einem bestimmten <see cref="T:System.Array" />-Index.
            /// </summary>
            /// <param name="array">
            ///     Das eindimensionale <see cref="T:System.Array" />, das das Ziel der aus
            ///     <see cref="T:System.Collections.Generic.ICollection`1" />
            ///     kopierten Elemente ist.Für <see cref="T:System.Array" /> muss eine nullbasierte Indizierung verwendet werden.
            /// </param>
            /// <param name="arrayIndex">
            ///     Der nullbasierte Index in <paramref name="array" />, an dem das Kopieren beginnt.
            /// </param>
            /// <exception cref="T:System.ArgumentNullException">
            ///     <paramref name="array" /> hat den Wert null.
            /// </exception>
            /// <exception cref="T:System.ArgumentOutOfRangeException">
            ///     <paramref name="arrayIndex" /> ist kleiner als 0.
            /// </exception>
            /// <exception cref="T:System.ArgumentException">
            ///     <paramref name="array" /> ist mehrdimensional.
            ///     - oder -Die Anzahl der Elemente in der Quelle <see cref="T:System.Collections.Generic.ICollection`1" />
            ///     ist größer als der verfügbare Speicherplatz ab <paramref name="arrayIndex" /> bis zum Ende des
            ///     <paramref name="array" />
            ///     ,
            ///     das als Ziel festgelegt wurde.- oder -Type TValue kann nicht automatisch in den Typ des Ziel-
            ///     <paramref name="array" />
            ///     umgewandelt werden.
            /// </exception>
            public void CopyTo(TValue[] array, int arrayIndex)
            {
                GetAll.ToArray().CopyTo(array, arrayIndex);
            }

            /// <summary>Gibt einen Enumerator zurück, der die Auflistung durchläuft.</summary>
            /// <returns>
            ///     Ein <see cref="T:System.Collections.Generic.IEnumerator`1" />, der zum Durchlaufen der Auflistung verwendet werden
            ///     kann.
            /// </returns>
            /// <filterpriority>1</filterpriority>
            public IEnumerator<TValue> GetEnumerator()
            {
                return GetAll.GetEnumerator();
            }

            /// <summary>
            ///     Entfernt das erste Vorkommen eines bestimmten Objekts aus <see cref="T:System.Collections.Generic.ICollection`1" />
            ///     .
            /// </summary>
            /// <returns>
            ///     True, wenn <paramref name="item" /> erfolgreich aus <see cref="T:System.Collections.Generic.ICollection`1" />
            ///     gelöscht wurde, andernfalls false.Diese Methode gibt auch dann false zurück, wenn
            ///     <paramref name="item" />
            ///     nicht in der ursprünglichen <see cref="T:System.Collections.Generic.ICollection`1" /> gefunden wurde.
            /// </returns>
            /// <param name="item">
            ///     Das aus dem <see cref="T:System.Collections.Generic.ICollection`1" /> zu entfernende Objekt.
            /// </param>
            /// <exception cref="T:System.NotSupportedException">
            ///     <see cref="T:System.Collections.Generic.ICollection`1" /> ist schreibgeschützt.
            /// </exception>
            public bool Remove(TValue item)
            {
                throw new NotSupportedException();
            }

            #endregion
        }

        #endregion Nested type: AllValueCollection

        [NotNull]
        private readonly SerializationInfo _info;

        /// <summary>The _list type.</summary>
        private readonly Type _listType;

        /// <summary>The _generic temp.</summary>
        private Type _genericTemp;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GroupDictionary{TKey,TValue}" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="GroupDictionary{TKey,TValue}" /> Klasse.
        /// </summary>
        /// <param name="listType">
        ///     The list type.
        /// </param>
        public GroupDictionary([NotNull] Type listType)
        {
            if (listType == null) throw new ArgumentNullException(nameof(listType));
            _listType = listType;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GroupDictionary{TKey,TValue}" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="GroupDictionary{TKey,TValue}" /> Klasse.
        /// </summary>
        public GroupDictionary()
        {
            _listType = typeof(List<TValue>);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GroupDictionary{TKey,TValue}" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="GroupDictionary{TKey,TValue}" /> Klasse.
        /// </summary>
        /// <param name="singleList">
        ///     The single list.
        /// </param>
        public GroupDictionary(bool singleList)
        {
            _listType = singleList ? typeof(HashSet<TValue>) : typeof(List<TValue>);
        }

        public GroupDictionary(GroupDictionary<TKey, TValue> groupDictionary)
            : base(groupDictionary)
        {
            _listType = groupDictionary._listType;
            _genericTemp = groupDictionary._genericTemp;
        }

        /// <summary>Gibt eine Collection zurück die Alle in den Listen enthaltenen Werte Darstellen.</summary>
        /// <value>The all values.</value>
        [NotNull]
        public ICollection<TValue> AllValues => new AllValueCollection(this);

        /// <summary>
        ///     Gibt eine Liste mit entsprechenden Schlüssel zurück. Ist keine Liste bkannt
        ///     wird eine erstellt.
        /// </summary>
        /// <param name="key">
        ///     Der Schlüssel nach dem gesucht werden soll.
        /// </param>
        /// <returns>
        ///     Eine Passende Collection.
        /// </returns>
        public new ICollection<TValue> this[[NotNull] TKey key]
        {
            get
            {
                if (!ContainsKey(key)) Add(key);

                return base[key];
            }

            set => base[key] = value;
        }

        /// <summary>The create list.</summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        /// <exception cref="InvalidOperationException"></exception>
        [NotNull]
        private object CreateList()
        {
            if (!typeof(ICollection<TValue>).IsAssignableFrom(_listType)) throw new InvalidOperationException();

            if (_genericTemp != null) return Activator.CreateInstance(_genericTemp);

            if (_listType.ContainsGenericParameters)
            {
                if (_listType.GetGenericArguments().Length != 1) throw new InvalidOperationException();

                _genericTemp = _listType.MakeGenericType(typeof(TValue));
            }
            else
            {
                var generic = _listType.GetGenericArguments();
                if (generic.Length > 1) throw new InvalidOperationException();

                if (generic.Length == 0) _genericTemp = _listType;

                if (_genericTemp == null && generic[0] == typeof(TValue)) _genericTemp = _listType;
                else _genericTemp = _listType.GetGenericTypeDefinition().MakeGenericType(typeof(TValue));
            }

            if (_genericTemp == null) throw new InvalidOperationException();

            return Activator.CreateInstance(_genericTemp);
        }

        /// <summary>
        ///     Fügt einen schlüssel zur liste hinzu.
        /// </summary>
        /// <param name="key">
        ///     Der Schlüssel der hinzugefügt werden soll.
        /// </param>
        public void Add([NotNull] TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (!ContainsKey(key)) base[key] = (ICollection<TValue>) CreateList();
        }

        /// <summary>
        ///     Fügt eineesn schlüssel und ein Element hinzu bei .
        /// </summary>
        /// <param name="key">
        ///     Der Schlüssel unter dem ein wert hinzugefügt werden soll.
        /// </param>
        /// <param name="value">
        ///     Der wert der Hinzugefügt werden soll.
        /// </param>
        public void Add([NotNull] TKey key, [NotNull] TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (!ContainsKey(key)) Add(key);

            var list = base[key];
            list?.Add(value);
        }

        /// <summary>
        ///     Fügt eine ganze liste von werten hinzu.
        /// </summary>
        /// <param name="key">
        ///     Der Schlüssel zu dem ein wert hinzugefügt werden soll.
        /// </param>
        /// <param name="value">
        ///     Die werte die hinzugefügt werden sollen.
        /// </param>
        public void AddRange([NotNull] TKey key, [NotNull] IEnumerable<TValue> value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (!ContainsKey(key)) Add(key);

            var values = base[key];
            if (values == null) return;
            foreach (var item in value.Where(item => item != null)) values.Add(item);
        }

        /// <summary>
        ///     Entfernt einen wert unabhänig vom schlüssel.
        /// </summary>
        /// <param name="value">
        ///     Der wert der entfernt werden soll.
        /// </param>
        /// <returns>
        ///     Ob der wert Entfernt werden konnte.
        /// </returns>
        public bool RemoveValue([NotNull] TValue value)
        {
            return RemoveImpl(null, value, false, true);
        }

        /// <summary>
        ///     Entfernt einen wert unabhänig vom schlüssel.
        /// </summary>
        /// <param name="value">
        ///     Der wert der entfernt werden soll.
        /// </param>
        /// <param name="removeEmptyLists">
        ///     Gibt an ob leere listen entfernt werden sollen.
        /// </param>
        /// <returns>
        ///     Ob der wert Entfernt werden konnte.
        /// </returns>
        public bool Remove([NotNull] TValue value, bool removeEmptyLists)
        {
            return RemoveImpl(null, value, removeEmptyLists, true);
        }

        /// <summary>
        ///     Entfernt einen Wert aus der Liste eines bestimten schlüssels.
        /// </summary>
        /// <param name="key">
        ///     Der schlüssel der den Wert enthält der Enfernt werden soll.
        /// </param>
        /// <param name="value">
        ///     Der wert der Enfernt werden soll.
        /// </param>
        /// <returns>
        ///     Ob der wert Enfernt werden konnte.
        /// </returns>
        public bool Remove([NotNull] TKey key, [NotNull] TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            return RemoveImpl(key, value, false, false);
        }

        /// <summary>
        ///     Entfernt einen Wert aus der Liste eines bestimten schlüssels.
        /// </summary>
        /// <param name="key">
        ///     Der schlüssel der den Wert enthält der Enfernt werden soll.
        /// </param>
        /// <param name="value">
        ///     Der wert der Enfernt werden soll.
        /// </param>
        /// <param name="removeListIfEmpty">
        ///     Gibt an ob alle leeren listen entfernt werden sollen.
        /// </param>
        /// <returns>
        ///     Ob der wert Enfernt werden konnte.
        /// </returns>
        public bool Remove([NotNull] TKey key, [NotNull] TValue value, bool removeListIfEmpty)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            return RemoveImpl(key, value, removeListIfEmpty, false);
        }

        /// <summary>
        ///     The remove impl.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <param name="val">
        ///     The val.
        /// </param>
        /// <param name="removeEmpty">
        ///     The remove empty.
        /// </param>
        /// <param name="removeAll">
        ///     The remove all.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private bool RemoveImpl([CanBeNull] TKey key, [CanBeNull] TValue val, bool removeEmpty, bool removeAll)
        {
            var ok = false;

            if (removeAll)
            {
                var keys = Keys.ToArray().GetEnumerator();
                var vals = Values.ToArray().GetEnumerator();
                while (keys.MoveNext() && vals.MoveNext())
                {
                    var coll = (ICollection<TValue>) vals.Current;
                    var currkey = (TKey) keys.Current;
                    ok |= RemoveList(coll, val);
                    if (removeEmpty && coll.Count == 0) ok |= Remove(currkey);
                }
            }

            #region Single

            else
            {
                ok = ContainsKey(key);
                if (!ok) return ok;
                var col = base[key];

                ok |= RemoveList(col, val);
                if (!removeEmpty) return ok;
                if (col.Count == 0) ok |= Remove(key);
            }

            #endregion Single

            return ok;
        }

        /// <summary>
        ///     The remove list.
        /// </summary>
        /// <param name="vals">
        ///     The vals.
        /// </param>
        /// <param name="val">
        ///     The val.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private static bool RemoveList([NotNull] ICollection<TValue> vals, [NotNull] TValue val)
        {
            if (vals == null) throw new ArgumentNullException(nameof(vals));
            if (val == null) throw new ArgumentNullException(nameof(val));
            var ok = false;
            while (vals.Remove(val)) ok = true;

            return ok;
        }

        #region Serializable

        /// <summary>
        ///     Implementiert die <see cref="T:System.Runtime.Serialization.ISerializable" />-Schnittstelle und gibt die zum
        ///     Serialisieren der
        ///     <see cref="T:System.Collections.Generic.Dictionary`2" />
        ///     -Instanz erforderlichen Daten zurück.
        /// </summary>
        /// <param name="info">
        ///     Ein <see cref="T:System.Runtime.Serialization.SerializationInfo" />-Objekt mit den zum Serialisieren der
        ///     <see cref="T:System.Collections.Generic.Dictionary`2" />
        ///     -Instanz erforderlichen Informationen.
        /// </param>
        /// <param name="context">
        ///     Eine <see cref="T:System.Runtime.Serialization.StreamingContext" />-Struktur, die die Quelle und das Ziel des
        ///     serialisierten Streams enthält, der der
        ///     <see cref="T:System.Collections.Generic.Dictionary`2" />
        ///     -Instanz zugeordnet ist.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="info" /> ist null.
        /// </exception>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("listType", _listType, typeof(Type));

            base.GetObjectData(info, context);
        }

#pragma warning disable 628

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="GroupDictionary{TKey,TValue}" /> Klasse.
        /// </summary>
        /// <param name="info">
        /// </param>
        /// <param name="context">
        /// </param>
        [SuppressMessage("Microsoft.Design", "CA1047:DoNotDeclareProtectedMembersInSealedTypes")]
        protected GroupDictionary([NotNull] SerializationInfo info, StreamingContext context)
#pragma warning restore 628
            : base(info, context)
        {
            _info = info;
            _listType = (Type) info.GetValue("listType", typeof(Type));
        }

        #endregion Serializable
    }
}