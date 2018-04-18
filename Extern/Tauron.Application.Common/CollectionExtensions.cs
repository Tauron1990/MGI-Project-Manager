#region

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

#endregion

namespace Tauron
{
    /// <summary>Stellt Erweiterung Methoden für Auflistungen zur Verfügung.</summary>
    [PublicAPI]
    public static class CollectionExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Fügt einen Wert hinzu wenn kein eintsprechender Schlüssel Vormanden ist und
        ///     gibt den Inhalt anschliesen zurück.
        /// </summary>
        /// <param name="dic">
        ///     Das Dicionary in das der Wert eingefügt werden soll.
        /// </param>
        /// <param name="key">
        ///     Der Schlussel der geprüft werden soll.
        /// </param>
        /// <param name="creator">
        ///     Eine Methode die den Inhalt bei bedarf erstellt.
        /// </param>
        /// <example>
        ///     Beispielhafte darstellung des Aufrufs <see cref="AddIfNotExis{TKey,TValue}" />
        ///     <code>
        /// var value = dic.(key, () =&gt; Creator());
        /// </code>
        /// </example>
        /// <typeparam name="TKey">
        ///     Der Type des Schlüssels.
        /// </typeparam>
        /// <typeparam name="TValue">
        ///     Der Type des Inhalts.
        /// </typeparam>
        /// <returns>
        ///     Der Wert, der entwerder erstellt wurde oder schon enthalten war.
        /// </returns>
        public static TValue AddIfNotExis<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dic,
                                                        [NotNull]      TKey                      key, [NotNull] Func<TValue> creator)
        {
            if (dic == null) throw new ArgumentNullException(nameof(dic));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (creator == null) throw new ArgumentNullException(nameof(creator));
            TValue temp;

            if (dic.ContainsKey(key))
            {
                temp = dic[key];
            }
            else
            {
                temp     = creator();
                dic[key] = temp;
            }

            return temp;
        }

        /// <summary>
        ///     Gibt den Inhalt eines Schlüssels von einem Dictionary&lt;string,object&gt; zurück und
        ///     Castet ihn in den richtigen Typen.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Gibt den Inhalt eines Schlüssels von einem Dictionary&lt;string,object&gt; zurück und
        ///         Castet ihn in den richtigen Typen.
        ///     </para>
        ///     <para>
        ///         Wenn kein Schlüssel vorhanden ist oder der Type Falsch ist wird null zurück gegeben.
        ///     </para>
        /// </remarks>
        /// <example>
        ///     Beispielhafte Darstellung eines Aufrufes.
        ///     <code>
        /// IDictionary dic = Initialize();
        /// Type sample = dic.TryGetAndCast&lt;Type&gt;("SampleKey");
        /// </code>
        /// </example>
        /// <param name="dic">
        ///     Das
        ///     <seealso cref="IDictionary{Key, Value}" />
        ///     , dessen Wert entnommen werden soll.
        /// </param>
        /// <param name="key">
        ///     Der Schlüssel nach dem Gesucht werden soll.
        /// </param>
        /// <typeparam name="TValue">
        ///     Der Type in dem der Wert gecastet werden soll.
        /// </typeparam>
        /// <returns>
        ///     Der gecastete wert oder null.
        /// </returns>
        [CanBeNull]
        public static TValue TryGetAndCast<TValue>([NotNull] this IDictionary<string, object> dic, [NotNull] string key)
            where TValue : class
        {
            if (dic == null) throw new ArgumentNullException(nameof(dic));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (dic.TryGetValue(key, out var obj)) return obj as TValue;

            return null;
        }

        #endregion
    }
}