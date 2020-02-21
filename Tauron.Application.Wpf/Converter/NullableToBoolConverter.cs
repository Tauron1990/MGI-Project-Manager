﻿using System.Windows.Data;
using System.Windows.Markup;

namespace Tauron.Application.Wpf.Converter
{
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    public sealed class NullableToBoolConverter : ValueConverterFactoryBase
    {
        protected override IValueConverter Create()
        {
            return new Converter();
        }

        private class Converter : ValueConverterBase<bool, bool?>
        {
            protected override bool CanConvertBack => true;

            protected override bool? Convert(bool value)
            {
                return value;
            }

            protected override bool ConvertBack(bool? value)
            {
                return value == true;
            }
        }
    }
}