using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using JetBrains.Annotations;

namespace Tauron.Application.Wpf.Converter
{
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    [DebuggerNonUserCode]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public abstract class ValueConverterFactoryBase : MarkupExtension
    {
        public IServiceProvider? ServiceProvider { get; set; }

        [NotNull]
        protected abstract IValueConverter Create();

        protected static IValueConverter CreateStringConverter<TType>(Func<TType, string> converter)
        {
            return new FuncStringConverter<TType>(converter);
        }

        protected static IValueConverter CreateCommonConverter<TSource, TDest>(Func<TSource, TDest> converter)
        {
            return new FuncCommonConverter<TSource, TDest>(converter);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;

            return Create();
        }

        private class FuncCommonConverter<TSource, TDest> : ValueConverterBase<TSource, TDest>
        {
            private readonly Func<TSource, TDest> _func;

            public FuncCommonConverter(Func<TSource, TDest> func)
            {
                _func = func;
            }

            protected override TDest Convert(TSource value)
            {
                return _func(value);
            }
        }

        private class FuncStringConverter<TType> : StringConverterBase<TType>
        {
            private readonly Func<TType, string> _converter;

            public FuncStringConverter(Func<TType, string> converter)
            {
                _converter = converter;
            }

            protected override string Convert(TType value)
            {
                return _converter(value);
            }
        }

        protected abstract class StringConverterBase<TSource> : ValueConverterBase<TSource, string>
        {
        }

        protected abstract class ValueConverterBase<TSource, TDest> : IValueConverter
        {
            protected virtual bool CanConvertBack => false;

            public virtual object? Convert(object value, [NotNull] Type targetType, object parameter, [NotNull] CultureInfo culture)
            {
                //if (value is TDest && typeof(TSource) != typeof(TDest)) return value;
                if (!(value is TSource)) return null;

                return Convert((TSource) value);
            }

            public virtual object? ConvertBack(object value, [NotNull] Type targetType, object parameter, [NotNull] CultureInfo culture)
            {
                if (!CanConvertBack || !(value is TDest)) return null;

                return ConvertBack((TDest) value);
            }

            protected abstract TDest Convert(TSource value);

            protected virtual TSource ConvertBack(TDest value)
            {
                return default!;
            }
        }
    }
}