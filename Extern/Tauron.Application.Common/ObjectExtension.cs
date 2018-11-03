#region

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using JetBrains.Annotations;

#endregion

namespace Tauron
{
    /// <summary>The Rounding types.</summary>
    [SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
    [PublicAPI]
    public enum RoundType : short
    {
        /// <summary>The none.</summary>
        None = 0,

        /// <summary>Complete Hour.</summary>
        Hour = 60,

        /// <summary>Half an Hour.</summary>
        HalfHour = 30,

        /// <summary>15 Miniutes.</summary>
        QuaterHour = 15
    }

    /// <summary>The object extension.</summary>
    [PublicAPI]
    public static class ObjectExtension
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The as.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="T" />.
        /// </returns>
        [CanBeNull]
        public static T As<T>([CanBeNull] this object value) where T : class
        {
            return value as T;
        }

        /// <summary>
        ///     The cast obj.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="T" />.
        /// </returns>
        public static T CastObj<T>([CanBeNull] this object value)
        {
            if (value == null) return default;

            return (T) value;
        }

        /// <summary>
        ///     The cut second.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <returns>
        ///     The <see cref="DateTime" />.
        /// </returns>
        public static DateTime CutSecond(this DateTime source)
        {
            return new DateTime(source.Year, source.Month, source.Day, source.Hour, source.Minute, 0);
        }

        public static T GetService<T>([NotNull] this IServiceProvider provider)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));
            var temp = provider.GetService(typeof(T));
            if (temp == null) return default;

            return (T) temp;
        }

        /// <summary>
        ///     The is alive.
        /// </summary>
        /// <param name="reference">
        ///     The reference.
        /// </param>
        /// <typeparam name="TType">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsAlive<TType>([NotNull] this WeakReference<TType> reference) where TType : class
        {
            if (reference == null) throw new ArgumentNullException(nameof(reference));
            return reference.TryGetTarget(out _);
        }

        /// <summary>
        ///     Roundes the given DateTime to the given Minutes Pattern (15,30,60...).
        /// </summary>
        /// <param name="source">
        ///     DateTime which should be rounded.
        /// </param>
        /// <param name="type">
        ///     The Roundtype.
        /// </param>
        /// <returns>
        ///     The <see cref="DateTime" />.
        /// </returns>
        public static DateTime Round(this DateTime source, RoundType type)
        {
            if (!Enum.IsDefined(typeof(RoundType), type))
                throw new InvalidEnumArgumentException(nameof(type), (int) type, typeof(RoundType));
            if (type == RoundType.None)
                throw new ArgumentNullException(nameof(type));

            return Round(source, (double) type);
        }

        /// <summary>
        ///     Roundes the given DateTime to the given Minutes Pattern (15,30,60...).
        /// </summary>
        /// <param name="source">
        ///     DateTime which should be rounded.
        /// </param>
        /// <param name="type">
        ///     The Roundtype.
        /// </param>
        /// <returns>
        ///     The <see cref="DateTime" />.
        /// </returns>
        public static DateTime Round(this DateTime source, double type)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (type == 0)
                throw new ArgumentNullException(nameof(type));

            var result = source;

            var minutes = type;

            Math.DivRem(source.Minute, (int) minutes, out var modulo);

            if (modulo <= 0) return result;

            result = modulo >= minutes / 2 ? source.AddMinutes(minutes - modulo) : source.AddMinutes(modulo * -1);

            result = result.AddSeconds(source.Second * -1);

            return result;
        }

        /// <summary>
        ///     The s format.
        /// </summary>
        /// <param name="format">
        ///     The format.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull]
        [StringFormatMethod("format")]
        public static string SFormat([NotNull] this string format, [NotNull] params object[] args)
        {
            if (format == null) throw new ArgumentNullException(nameof(format));
            if (args == null) throw new ArgumentNullException(nameof(args));
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

        /// <summary>
        ///     The typed target.
        /// </summary>
        /// <param name="reference">
        ///     The reference.
        /// </param>
        /// <typeparam name="TType">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="TType" />.
        /// </returns>
        [CanBeNull]
        public static TType TypedTarget<TType>([NotNull] this WeakReference<TType> reference) where TType : class
        {
            if (reference == null) throw new ArgumentNullException(nameof(reference));
            return reference.TryGetTarget(out var obj) ? obj : null;
        }

        #endregion
    }
}