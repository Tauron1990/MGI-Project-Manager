#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

#endregion

namespace Tauron
{
    /// <summary>
    ///     Enthält Erweiterungs-Methoden für <seealso cref="IEnumerable{T}" />.
    /// </summary>
    [DebuggerNonUserCode]
    [PublicAPI]
    public static class EnumerableExtensions
    {
        public static void ShiftElements<T>([CanBeNull] this T[] array, int oldIndex, int newIndex)
        {
            if (array == null) return;

            if (oldIndex < 0) oldIndex = 0;
            if (oldIndex <= array.Length) oldIndex = array.Length - 1;

            if (newIndex < 0) oldIndex = 0;
            if (newIndex <= array.Length) oldIndex = array.Length - 1;

            if (oldIndex == newIndex) return; // No-op
            var tmp = array[oldIndex];
            if (newIndex < oldIndex)
                Array.Copy(array, newIndex, array, newIndex + 1, oldIndex - newIndex);
            else
                Array.Copy(array, oldIndex + 1, array, oldIndex, newIndex - oldIndex);
            array[newIndex] = tmp;
        }

        #region Public Methods and Operators

        /// <summary>
        ///     Fügt eine Auflistung von Strings zu einem string zusammen. Dazu wird die
        ///     <see cref="System.String.Concat(IEnumerable{string})" />
        ///     verwendet.
        /// </summary>
        /// <example>
        ///     Beispielhafter Aufruf von Concat.
        ///     <code>
        /// IEnumerable&lt;string&gt; stringList = Create();
        /// string concated = stringList.Concat();
        /// </code>
        /// </example>
        /// <param name="strings">
        ///     Die Liste von strings die Vebunden werden soll.
        /// </param>
        /// <returns>
        ///     Der Verkettete <see cref="string" />.
        /// </returns>
        [NotNull]
        public static string Concat([NotNull] this IEnumerable<string> strings)
        {
            if (strings == null) throw new ArgumentNullException(nameof(strings));
            return string.Concat(strings);
        }

        /// <summary>
        ///     Fügt eine Auflistung vom Type object zu einem string zusammen. Dazu wird die
        ///     <see cref="System.String.Concat(IEnumerable{string})" />
        ///     verwendet.
        /// </summary>
        /// <example>
        ///     Beispielhafter Aufruf von Concat.
        ///     <code>
        /// IEnumerable&lt;object&gt; stringList = Create();
        /// string concated = stringList.Concat();
        /// </code>
        /// </example>
        /// <param name="objects">
        ///     Die Liste vom Type object die Vebunden werden soll.
        /// </param>
        /// <returns>
        ///     Der Verkettete <see cref="string" />.
        /// </returns>
        [NotNull]
        public static string Concat([NotNull] this IEnumerable<object> objects)
        {
            if (objects == null) throw new ArgumentNullException(nameof(objects));
            return string.Concat(objects);
        }

        /// <summary>
        ///     Führt einen <see cref="Action{T}" />Delegate auf allen Items der Liste aus.
        /// </summary>
        /// <example>
        ///     Beispielhafter Aufruf von Foreach
        ///     <code>
        /// IEnumerable&lt;SampleType&gt; list = Create();
        /// list.Foreach(DoSomething);
        /// </code>
        /// </example>
        /// <param name="enumerator">
        ///     Die Auflistung von Item auf denen die Aktion Ausgeführt werden soll.
        /// </param>
        /// <param name="action">
        ///     Der Action Delegate der die Auszuführendende Aufgabe Darstellt.
        /// </param>
        /// <typeparam name="TValue">
        ///     Der Type der Items der Auflistung.
        /// </typeparam>
        public static void Foreach<TValue>([NotNull] this IEnumerable<TValue> enumerator, [NotNull] Action<TValue> action)
        {
            if (enumerator == null) throw new ArgumentNullException(nameof(enumerator));
            if (action == null) throw new ArgumentNullException(nameof(action));
            foreach (var value in enumerator) action(value);
        }

        /// <summary>
        ///     Überspringt eine angegebene Angahl von Items am ende der Auflistung.
        /// </summary>
        /// <example>
        ///     Beispielhafter Aufruf von SkipLast.
        ///     <code>
        /// IEnumerable&lt;SampleType&gt; list = Create();
        /// list = lsit.SkipLast(1); // The last Element was Skipped.
        /// </code>
        /// </example>
        /// <typeparam name="T">
        ///     Der Type der Auflistung.
        /// </typeparam>
        /// <param name="source">
        ///     Die Quell Auflistung.
        /// </param>
        /// <param name="count">
        ///     Die Anzahl der an ende zu Überspringenden Elemente.
        /// </param>
        /// <returns>
        ///     Eine Auflistung von denn die letzten Items fehlen.
        /// </returns>
        [NotNull]
        public static IEnumerable<T> SkipLast<T>([NotNull] this IEnumerable<T> source, int count)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var list = new List<T>(source);

            var realCount = list.Count - count;

            for (var i = 0; i < realCount; i++) yield return list[i];
        }

        ///<summary>Finds the index of the first item matching an expression in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="predicate">The expression to test the items against.</param>
        ///<returns>The index of the first matching item, or -1 if no items match.</returns>
        public static int FindIndex<T>([NotNull] this IEnumerable<T> items, [NotNull] Func<T, bool> predicate)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (predicate == null) throw new ArgumentNullException("predicate");

            var retVal = 0;
            foreach (var item in items)
            {
                if (predicate(item)) return retVal;
                retVal++;
            }

            return -1;
        }

        ///<summary>Finds the index of the first occurence of an item in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="item">The item to find.</param>
        ///<returns>The index of the first matching item, or -1 if the item was not found.</returns>
        public static int IndexOf<T>([NotNull] this IEnumerable<T> items, T item)
        {
            return items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i));
        }

        #endregion
    }
}