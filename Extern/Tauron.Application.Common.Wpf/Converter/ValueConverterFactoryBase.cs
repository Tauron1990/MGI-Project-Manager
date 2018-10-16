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
        #region Public Properties

        [CanBeNull] public IServiceProvider ServiceProvider { get; set; }

        #endregion

        #region Public Methods and Operators

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

        #endregion

        #region Methods

        [NotNull]
        protected abstract IValueConverter Create();

        #endregion

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

            #endregion
        }
    }
}