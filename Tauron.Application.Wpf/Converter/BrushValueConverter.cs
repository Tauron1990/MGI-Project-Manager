using System;
using System.Windows.Data;
using System.Windows.Media;
using JetBrains.Annotations;

namespace Tauron.Application.Wpf.Converter
{
    public sealed class BrushValueConverter : ValueConverterFactoryBase
    {
        protected override IValueConverter Create()
        {
            return new Converter();
        }

        private class Converter : ValueConverterBase<string, Brush>
        {
            private static readonly BrushConverter ConverterImpl = new BrushConverter();

            protected override Brush Convert([NotNull] string value)
            {
                try
                {
                    if (ConverterImpl.ConvertFrom(value) is Brush brush)
                        return brush;
                    return Brushes.Black;
                }
                catch (FormatException)
                {
                    return Brushes.Black;
                }
            }
        }
    }
}