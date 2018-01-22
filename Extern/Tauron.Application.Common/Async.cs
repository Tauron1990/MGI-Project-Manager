#region

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

#endregion

namespace Tauron
{
    /// <summary>Ermöglich auf einfache weise eine Asyncrone Aufgabe Auszuführen.</summary>
    /// <remarks>
    ///     <para>Ermöglich auf einfache weise eine Asyncrone Aufgabe Auszuführen.</para>
    ///     <para>Intern wird auf die TPL zurüggeriffen.</para>
    /// </remarks>
    [PublicAPI]
    public static class Async
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Startet einen neuen Task mit dem Standart Verhalten.
        /// </summary>
        /// <example>
        ///     Beschpielhafte darstellen des Aufruhfes <see cref="StartNew" />
        ///     <code>
        /// Task task = new Action(DoSonmthing).StartNew();
        /// </code>
        /// </example>
        /// <param name="method">
        ///     Ein Action Delegate das dem neuen
        ///     <seealso cref="Task" />
        ///     übergeben wird.
        /// </param>
        /// <returns>
        ///     Der erstellte <see cref="Task" />.
        /// </returns>
        public static Task StartNew([NotNull] this Action method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            return Task.Factory.StartNew(method);
        }

        /// <summary>
        ///     Startet einen neuen Task mit der Long Running Einstellung.
        /// </summary>
        /// <example>
        ///     Beschpielhafte darstellen des Aufruhfes <see cref="StartNewLong" />
        ///     <code>
        /// Task task = new Action(DoSonmthing).StartNewLong();
        /// </code>
        /// </example>
        /// <param name="method">
        ///     Ein Action Delegate das dem neuen
        ///     <seealso cref="Task" />
        ///     übergeben wird.
        /// </param>
        /// <returns>
        ///     Der erstellte <see cref="Task" />.
        /// </returns>
        public static Task StartNewLong([NotNull] this Action method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            return Task.Factory.StartNew(method, TaskCreationOptions.LongRunning);
        }

        #endregion
    }
}