#region

using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using JetBrains.Annotations;

#endregion

namespace Tauron.Application.Converter
{
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    [DebuggerNonUserCode]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public abstract class ValueConverterFactoryBase : MarkupExtension
    {
        private class FuncCommonConverter<TSource, TDest> : ValueConverterBase<TSource, TDest>
        {
            private readonly Func<TSource, TDest> _func;

            public FuncCommonConverter(Func<TSource, TDest> func)
            {
                _func = func;
            }

            protected override TDest Convert(TSource value) => _func(value);
        }

        private class FuncStringConverter<TType> : StringConverterBase<TType>
        {
            private readonly Func<TType, string> _converter;

            public FuncStringConverter(Func<TType, string> converter) => _converter = converter;

            protected override string Convert(TType value) => _converter(value);
        }

        protected abstract class StringConverterBase<TSource> : ValueConverterBase<TSource, string>
        {
        }

        /// <summary>
        ///     The value converter base.
        /// </summary>
        /// <typeparam name="TSource">
        /// </typeparam>
        /// <typeparam name="TDest">
        /// </typeparam>
        protected abstract class ValueConverterBase<TSource, TDest> : IValueConverter
        {
            #region Properties

            protected virtual bool CanConvertBack => false;

            #endregion

            #region Public Methods and Operators

            [CanBeNull]
            public virtual object Convert([NotNull] object value, [NotNull] Type targetType, [NotNull] object parameter,
                [NotNull] CultureInfo culture)
            {
                if (value is TDest && typeof(TSource) != typeof(TDest)) return value;
                if (!(value is TSource)) return null;

                return Convert((TSource) value);
            }

            [CanBeNull]
            public virtual object ConvertBack([NotNull] object value, [NotNull] Type targetType,
                [NotNull] object parameter, [NotNull] CultureInfo culture)
            {
                if (!CanConvertBack || !(value is TDest)) return null;

                return ConvertBack((TDest) value);
            }

            #endregion

            #region Methods

            protected abstract TDest Convert(TSource value);

            protected virtual TSource ConvertBack(TDest value)
            {
                return default(TSource);
            }

        #region Public Properties

        [CanBeNull]
        public IServiceProvider ServiceProvider { get; set; }

        #endregion

        #region Public Methods and Operators

        protected static IValueConverter CreateStringConverter<TType>(Func<TType, string> converter) => new FuncStringConverter<TType>(converter);

        protected static IValueConverter CreateCommonConverter<TSource, TDest>(Func<TSource, TDest> converter) => new FuncCommonConverter<TSource, TDest>(converter);

        /// <summary>
        ///     Gibt bei der Implementierung in einer abgeleiteten Klasse ein Objekt zurück, das als Wert der Zieleigenschaft für
        ///     die Markuperweiterung festgelegt wird.
        /// </summary>
        /// <returns>
        ///     Der Objektwert, der für die Eigenschaft festgelegt werden soll, für die die Erweiterung angewendet wird.
        /// </returns>
        /// <param name="serviceProvider">
        ///     Objekt, das Dienste für die Markuperweiterung bereitstellen kann.
        /// </param>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;

            return Create();
        }
    }
}