#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainerExtensions.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The container extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Tauron.Application.Ioc;

#endregion

namespace Tauron
{
    /// <summary>Stellt Erweiterung Methoden für den Ioc Container zur Verfügung.</summary>
    /// <remarks>
    ///     <para>Die Konkreteren methoden zum Abrufen von Instanzen werden mit Erweiterung Methoden Implementiert.</para>
    ///     <para>Dadurch bleibt das Container Interface Sauber.</para>
    /// </remarks>
    [PublicAPI]
    public static class ContainerExtensions
    {
        #region Constants

        private const string ErrorMessage = "Error on Return of Container Resolve";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Ruft einene Instanz aus dem Build System des Containers ab.
        /// </summary>
        /// <example>
        ///     Beispilhafter Aufruf der Methode.
        ///     <code>
        /// IContainer conatainer = Create();
        /// Service instanz = container.Resolve&lt;Service&gt;();
        /// </code>
        /// </example>
        /// <param name="con">
        ///     Der Container mit dem die Instanz abgerufen werden soll.
        /// </param>
        /// <typeparam name="TType">
        ///     Der Type der Abgerufen werden soll.
        /// </typeparam>
        /// <returns>
        ///     Eine Instanz von <see cref="TType" />.
        /// </returns>
        /// <exception cref="Tauron.Application.Ioc.BuildUpException">
        ///     Wird geworfen wenn Beim erstellen des Export ein Fehel aufgetreten ist.
        ///     Weiter informationen stehen dann in der Inner Exceptionzur verfügung.
        /// </exception>
        /// <exception cref="Tauron.Application.Ioc.FindExportException">
        ///     Wird geworfen wenn ein Export nicht gefunden wurde.
        ///     Weiter informationen stehen dann in der Inner Exceptionzur verfügung.
        /// </exception>
        [NotNull]
        public static TType Resolve<TType>([NotNull] this IContainer con, params BuildParameter[] parameters) where TType : class
        {
            if (con == null) throw new ArgumentNullException(nameof(con));
            return (TType) con.Resolve(typeof(TType), null, parameters);
        }

        /// <summary>
        ///     The resolve.
        /// </summary>
        /// <param name="con">
        ///     The con.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="optional">
        ///     The optional.
        /// </param>
        /// <typeparam name="TType">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TType" />.
        /// </returns>
        public static TType Resolve<TType>([NotNull] this IContainer con, string name, bool optional, params BuildParameter[] buildParameters) where TType : class
        {
            if (con == null) throw new ArgumentNullException(nameof(con));
            return con.Resolve(typeof(TType), name, optional, buildParameters) as TType;
        }

        /// <summary>
        ///     Ruft einene Instanz mit Vertragsnamen aus dem Build System des Containers ab.
        /// </summary>
        /// <example>
        ///     Beispilhafter Aufruf der Methode.
        ///     <code>
        /// IContainer conatainer = Create();
        /// Service instanz = container.Resolve(typeof(Service), "VertragsName");
        /// </code>
        /// </example>
        /// <param name="con">
        ///     Der Container mit dem die Instanz abgerufen werden soll.
        /// </param>
        /// <param name="interface">
        ///     Der Type der Abgerufen werden soll.
        /// </param>
        /// <param name="name">
        ///     Der Vertrags Namen für den Export.
        /// </param>
        /// <returns>
        ///     Eine Instanz des Exports.
        /// </returns>
        /// <exception cref="Tauron.Application.Ioc.BuildUpException">
        ///     Wird geworfen wenn Beim erstellen des Export ein Fehel aufgetreten ist.
        ///     Weiter informationen stehen dann in der Inner Exceptionzur verfügung.
        /// </exception>
        /// <exception cref="Tauron.Application.Ioc.FindExportException">
        ///     Wird geworfen wenn ein Export nicht gefunden wurde.
        ///     Weiter informationen stehen dann in der Inner Exceptionzur verfügung.
        /// </exception>
        public static object Resolve([NotNull] this IContainer con, [NotNull] Type @interface, string name, params BuildParameter[] buildParameters)
        {
            if (con == null) throw new ArgumentNullException(nameof(con));
            if (@interface == null) throw new ArgumentNullException(nameof(@interface));
            var tracer = new ErrorTracer();

            try
            {
                var expo = con.FindExport(@interface, name, tracer);
                return tracer.Exceptional ? null : con.BuildUp(expo, tracer, buildParameters);
            }
            finally
            {
                if (tracer.Exceptional)
                    throw new BuildUpException(tracer);
            }
        }

        /// <summary>
        ///     Ruft einene Instanz mit Vertragsnamen aus dem Build System des Containers ab
        ///     und gibt null zurück wenn kein Export gefunden wurde.
        /// </summary>
        /// <example>
        ///     Beispilhafter Aufruf der Methode.
        ///     <code>
        /// IContainer conatainer = Create();
        /// Service instanz = container.Resolve(typeof(Service), "VertragsName", true);
        /// </code>
        /// </example>
        /// <param name="con">
        ///     Der Container mit dem die Instanz abgerufen werden soll.
        /// </param>
        /// <param name="interface">
        ///     Der Type der Abgerufen werden soll.
        /// </param>
        /// <param name="name">
        ///     Der Vertrags Namen für den Export.
        /// </param>
        /// <param name="optional">
        ///     Gibt an ob der Export Optional ist oder nicht.
        /// </param>
        /// <returns>
        ///     Eine Instanz des Exports.
        /// </returns>
        /// <exception cref="Tauron.Application.Ioc.BuildUpException">
        ///     Wird geworfen wenn Beim erstellen des Export ein Fehel aufgetreten ist.
        ///     Weiter informationen stehen dann in der Inner Exceptionzur verfügung.
        /// </exception>
        /// <exception cref="Tauron.Application.Ioc.FindExportException">
        ///     Wird geworfen wenn ein Export nicht gefunden wurde.
        ///     Weiter informationen stehen dann in der Inner Exceptionzur verfügung.
        /// </exception>
        public static object Resolve([NotNull] this IContainer con, [NotNull] Type @interface, string name, bool optional, BuildParameter[] buildParameters)
        {
            if (con == null) throw new ArgumentNullException(nameof(con));
            if (@interface == null) throw new ArgumentNullException(nameof(@interface));
            var tracer = new ErrorTracer();

            try
            {
                var data = con.FindExport(@interface, name, tracer, optional);

                if (tracer.Exceptional) return null;
                if (data != null) return con.BuildUp(data, tracer, buildParameters);

                if (optional) return null;
                return null;
            }
            finally
            {
                if (tracer.Exceptional)
                    throw new BuildUpException(tracer);
            }
        }

        /// <summary>
        ///     Ruft alle Instanzen aus dem Build System des Containers ab.
        /// </summary>
        /// <example>
        ///     Beispilhafter Aufruf der Methode.
        ///     <code>
        /// IContainer conatainer = Create();
        /// IEnumerable&lt;object&gt; instanz = container.ResolveAll(typeof(Service), "VertragsName");
        /// </code>
        /// </example>
        /// <param name="con">
        ///     Der Container mit dem die Instanz abgerufen werden soll.
        /// </param>
        /// <param name="interface">
        ///     Der Type der Abgerufen werden soll.
        /// </param>
        /// <param name="name">
        ///     Der Vertrags Namen für den Export.
        /// </param>
        /// <returns>
        ///     Eine Auflistung von Instanzen des Exports als <see cref="IEnumerable{TType}" />.
        /// </returns>
        /// <exception cref="Tauron.Application.Ioc.BuildUpException">
        ///     Wird geworfen wenn Beim erstellen des Export ein Fehel aufgetreten ist.
        ///     Weiter informationen stehen dann in der Inner Exceptionzur verfügung.
        /// </exception>
        /// <exception cref="Tauron.Application.Ioc.FindExportException">
        ///     Wird geworfen wenn ein Export nicht gefunden wurde.
        ///     Weiter informationen stehen dann in der Inner Exceptionzur verfügung.
        /// </exception>
        public static IEnumerable<object> ResolveAll([NotNull] this IContainer con, [NotNull] Type @interface, string name, params BuildParameter[] buildParameters)
        {
            if (con == null) throw new ArgumentNullException(nameof(con));
            if (@interface == null) throw new ArgumentNullException(nameof(@interface));
            var tracer = new ErrorTracer();

            try
            {
                var temp = con.FindExports(@interface, name, tracer);
                if (tracer.Exceptional) yield break;

                foreach (var tempBuild in temp.Select(exportMetadata => con.BuildUp(exportMetadata, tracer, buildParameters)).TakeWhile(tempBuild => !tracer.Exceptional))
                    yield return tempBuild;
            }
            finally
            {
                if (tracer.Exceptional)
                    throw new BuildUpException(tracer);
            }
        }

        /// <summary>
        ///     The resolve all.
        /// </summary>
        /// <param name="con">
        ///     The con.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <typeparam name="TType">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        public static IEnumerable<TType> ResolveAll<TType>([NotNull] this IContainer con, string name)
        {
            if (con == null) throw new ArgumentNullException(nameof(con));
            return ResolveAll(con, typeof(TType), name).Cast<TType>();
        }

        #endregion
    }
}