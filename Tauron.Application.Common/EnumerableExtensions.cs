using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace Tauron
{
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
                System.Array.Copy(array, newIndex, array, newIndex + 1, oldIndex - newIndex);
            else
                System.Array.Copy(array, oldIndex + 1, array, oldIndex, newIndex - oldIndex);
            array[newIndex] = tmp;
        }

        public static string Concat(this IEnumerable<string> strings) 
            => string.Concat(strings);

        public static string Concat([NotNull] this IEnumerable<object> objects) 
            => string.Concat(objects);

        public static void Foreach<TValue>(this IEnumerable<TValue> enumerator, [NotNull] Action<TValue> action)
        {
            foreach (var value in enumerator) 
                action(value);
        }

        public static IEnumerable<T> SkipLast<T>([NotNull] this IEnumerable<T> source, int count)
        {
            var list = new List<T>(source);

            var realCount = list.Count - count;

            for (var i = 0; i < realCount; i++) 
                yield return list[i];
        }

        public static int FindIndex<T>([NotNull] this IEnumerable<T> items, Func<T, bool> predicate)
        {
            var retVal = 0;
            foreach (var item in items)
            {
                if (predicate(item)) return retVal;
                retVal++;
            }

            return -1;
        }
        
        public static int IndexOf<T>([NotNull] this IEnumerable<T> items, T item) => items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i));
        
        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
                yield return array.Skip(i * size).Take(size);
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this List<T> array, int size)
        {
            for (var i = 0; i < (float)array.Count / size; i++)
                yield return array.Skip(i * size).Take(size);
        }

        public static int Count(this IEnumerable source)
        {
            if (source is ICollection col)
                return col.Count;

            var c = 0;
            var e = source.GetEnumerator();
            e.DynamicUsing(() =>
            {
                while (e.MoveNext())
                    c++;
            });

            return c;
        }
    }
}