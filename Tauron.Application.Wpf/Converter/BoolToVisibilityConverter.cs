using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using JetBrains.Annotations;

namespace Tauron.Application.Wpf.Converter
{
    [PublicAPI]
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    public class StringToIntConverter : ValueConverterFactoryBase
    {
        protected override IValueConverter Create() => new Converter();

        private class Converter : StringConverterBase<int>
        {
            protected override bool CanConvertBack => true;

            protected override string Convert(int value) => value.ToString();

            protected override int ConvertBack(string value)
            {
                if (string.IsNullOrEmpty(value))
                    return 0;

                try
                {
                    return int.Parse(value);
                }
                catch (Exception e) when (e is ArgumentException || e is FormatException || e is OverflowException)
                {
                    return 0;
                }
            }
        }
    }


    [PublicAPI]
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    public class BoolToVisibilityConverter : ValueConverterFactoryBase
    {
        public bool IsHidden { get; set; }

        public bool Reverse { get; set; }

        protected override IValueConverter Create() => new Converter(IsHidden, Reverse);

        private class Converter : ValueConverterBase<bool, Visibility>
        {
            private readonly bool _isHidden;

            private readonly bool _reverse;

            public Converter(bool isHidden, bool reverse)
            {
                _isHidden = isHidden;
                _reverse = reverse;
            }

            protected override bool CanConvertBack => true;

            protected override Visibility Convert(bool value)
            {
                if (_reverse) value = !value;

                if (value) return Visibility.Visible;

                return _isHidden ? Visibility.Hidden : Visibility.Collapsed;
            }

            protected override bool ConvertBack(Visibility value)
            {
                bool result;
                switch (value)
                {
                    case Visibility.Collapsed:
                    case Visibility.Hidden:
                        result = false;
                        break;
                    case Visibility.Visible:
                        result = true;
                        break;
                    default:
                        result = false;
                        break;
                }

                if (_reverse) result = !result;

                return result;
            }
        }
    }
}